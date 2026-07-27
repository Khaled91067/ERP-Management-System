import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { SuppliersService, Supplier } from '../suppliers.service';
import { NotificationService } from '@core/services/notification.service';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-suppliers-list',
  standalone: true,
  imports: [
    RouterLink,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatMenuModule,
    MatTooltipModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Suppliers"
        description="Manage suppliers, contacts and purchasing partners."
        [breadcrumbs]="[{ label: 'Purchasing' }, { label: 'Suppliers' }]"
        actionLabel="New Supplier"
        actionIcon="add"
        (action)="router.navigate(['/admin/suppliers/new'])"
      />

      <div class="feature-card">
        <div class="feature-card__header">
          <div class="feature-card__title">
            Suppliers <span class="feature-card__count">({{ totalItems() }})</span>
          </div>
          <div class="table-toolbar">
            <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
              <mat-icon matPrefix>search</mat-icon>
              <input matInput [formControl]="searchControl" placeholder="Search suppliers...">
              <span matSuffix class="search-shortcut">⌘K</span>
            </mat-form-field>
          </div>
        </div>

        <div class="table-wrapper">
          <table mat-table [dataSource]="isLoading() ? skeletonData : suppliers()" class="full-width">
            
            <ng-container matColumnDef="company">
              <th mat-header-cell *matHeaderCellDef>Company</th>
              <td mat-cell *matCellDef="let element">
                @if (isLoading()) {
                  <div class="skeleton-line w-32"></div>
                  <div class="skeleton-line w-16 mt-2"></div>
                } @else {
                  <div class="supplier-info">
                    <span class="company-name">{{ element.companyName }}</span>
                    <span class="badge badge-neutral mt-1">Terms: {{ element.paymentTerms || 'N/A' }}</span>
                  </div>
                }
              </td>
            </ng-container>

            <ng-container matColumnDef="contact">
              <th mat-header-cell *matHeaderCellDef>Contact Info</th>
              <td mat-cell *matCellDef="let element">
                @if (isLoading()) {
                  <div class="flex-row contact-info">
                    <div class="skeleton-avatar"></div>
                    <div>
                      <div class="skeleton-line w-24"></div>
                      <div class="skeleton-line w-32 mt-2"></div>
                    </div>
                  </div>
                } @else {
                  <div class="contact-info flex-row">
                    <div class="contact-avatar">{{ getInitials(element.contactName) }}</div>
                    <div class="contact-details">
                      <span class="contact-name">{{ element.contactName }}</span>
                      <span class="contact-email">{{ element.email }}</span>
                    </div>
                  </div>
                }
              </td>
            </ng-container>

            <ng-container matColumnDef="phone">
              <th mat-header-cell *matHeaderCellDef>Phone</th>
              <td mat-cell *matCellDef="let element">
                @if (isLoading()) {
                  <div class="skeleton-line w-24"></div>
                } @else {
                  {{ element.phone }}
                }
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef class="actions-column"></th>
              <td mat-cell *matCellDef="let element" class="actions-column">
                @if (!isLoading()) {
                  <button mat-icon-button [matMenuTriggerFor]="menu" class="action-menu-btn">
                    <mat-icon>more_horiz</mat-icon>
                  </button>
                  <mat-menu #menu="matMenu" xPosition="before">
                    <button mat-menu-item [routerLink]="['/admin/suppliers', element.id, 'edit']">
                      <mat-icon>edit</mat-icon>
                      <span>Edit</span>
                    </button>
                    <button mat-menu-item class="danger-menu-item" (click)="deleteSupplier(element)">
                      <mat-icon>delete</mat-icon>
                      <span>Delete</span>
                    </button>
                  </mat-menu>
                }
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="columns"></tr>
            <tr mat-row *matRowDef="let row; columns: columns;"></tr>
          </table>

          @if (!isLoading() && suppliers().length === 0) {
            <div class="table-empty-state">
              <mat-icon>store</mat-icon>
              <span class="empty-title">No suppliers found.</span>
              <span class="empty-subtitle">Create your first supplier to start managing purchasing.</span>
              <button mat-flat-button color="primary" class="mt-4" (click)="router.navigate(['/admin/suppliers/new'])">
                New Supplier
              </button>
            </div>
          }
        </div>

        <div class="pagination-footer">
          <mat-paginator
            [length]="totalItems()"
            [pageSize]="pageSize()"
            [pageIndex]="pageIndex()"
            [pageSizeOptions]="[10, 20, 50]"
            (page)="onPageChange($event)"
            showFirstLastButtons>
          </mat-paginator>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .feature-card {
      background-color: var(--surface-card);
      border-radius: var(--radius-card);
      border: 1px solid var(--border-default);
      box-shadow: var(--shadow-sm);
      overflow: hidden;
      display: flex;
      flex-direction: column;
    }

    .feature-card__header {
      padding: 16px 20px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      border-bottom: 1px solid var(--border-divider);
    }

    .feature-card__title {
      font-size: 1rem;
      font-weight: 600;
      color: var(--text-primary);
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .feature-card__count {
      font-size: 0.875rem;
      color: var(--text-tertiary);
      font-weight: 400;
    }

    .table-toolbar {
      display: flex;
    }

    .search-field {
      width: 280px;
      --mdc-outlined-text-field-container-shape: 16px;
    }

    .search-field ::ng-deep .mdc-text-field {
      padding-right: 12px;
    }

    .search-shortcut {
      font-size: 0.6875rem;
      color: var(--text-tertiary);
      border: 1px solid var(--border-divider);
      padding: 2px 6px;
      border-radius: 4px;
      font-weight: 500;
      pointer-events: none;
    }

    .table-wrapper {
      width: 100%;
      overflow-x: auto;
    }

    .full-width {
      width: 100%;
    }

    .supplier-info {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
    }

    .company-name {
      font-weight: 500;
      color: var(--text-primary);
    }

    .mt-1 { margin-top: 4px; }
    .mt-2 { margin-top: 8px; }
    .mt-4 { margin-top: 16px; }

    .contact-info {
      gap: 12px;
    }

    .contact-avatar {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      background-color: var(--primary-light);
      color: var(--primary-base);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.75rem;
      font-weight: 600;
      flex-shrink: 0;
    }

    .contact-details {
      display: flex;
      flex-direction: column;
    }

    .contact-name {
      font-weight: 500;
      color: var(--text-primary);
    }

    .contact-email {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }

    .actions-column {
      width: 60px;
      padding-right: 8px !important;
      text-align: right;
    }

    .action-menu-btn {
      color: var(--text-secondary);
    }

    .danger-menu-item:hover {
      background-color: var(--status-error-bg) !important;
      color: var(--status-error-text) !important;
    }
    .danger-menu-item:hover mat-icon {
      color: var(--status-error-text) !important;
    }

    .pagination-footer {
      padding: 4px 8px;
    }

    /* Minimal Paginator */
    ::ng-deep .mat-mdc-paginator-container {
      min-height: 48px;
      justify-content: flex-end;
    }
    ::ng-deep .mat-mdc-paginator-range-label {
      margin: 0 24px;
    }
    ::ng-deep .mat-mdc-paginator-page-size {
      margin-right: 24px;
    }

    /* Skeleton Loading */
    .skeleton-line {
      height: 12px;
      background: var(--border-divider);
      border-radius: 4px;
      animation: pulse 1.5s infinite;
    }
    .skeleton-avatar {
      width: 32px;
      height: 32px;
      background: var(--border-divider);
      border-radius: 50%;
      animation: pulse 1.5s infinite;
    }
    .w-32 { width: 128px; }
    .w-24 { width: 96px; }
    .w-16 { width: 64px; }

    @keyframes pulse {
      0% { opacity: 0.6; }
      50% { opacity: 0.3; }
      100% { opacity: 0.6; }
    }
  `]
})
export class SuppliersListComponent implements OnInit {
  private readonly suppliersService = inject(SuppliersService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['company', 'contact', 'phone', 'actions'];
  readonly suppliers = signal<Supplier[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly skeletonData = [{}, {}, {}, {}, {}] as any;
  readonly searchControl = new FormControl('');

  ngOnInit(): void {
    this.loadSuppliers();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadSuppliers();
      });
  }

  loadSuppliers(): void {
    this.isLoading.set(true);
    this.suppliersService.getSuppliers({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined
    }).subscribe({
      next: (result) => {
        this.suppliers.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load suppliers');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadSuppliers();
  }

  deleteSupplier(supplier: Supplier): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Supplier',
        message: 'Are you sure you want to delete this supplier?',
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.suppliersService.deleteSupplier(supplier.id).subscribe({
          next: () => {
            this.notification.success('Supplier deleted successfully');
            this.loadSuppliers();
          },
          error: () => this.notification.error('Failed to delete supplier')
        });
      }
    });
  }

  getInitials(name: string): string {
    if (!name) return '';
    return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
  }
}
