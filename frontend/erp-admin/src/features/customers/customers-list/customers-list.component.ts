import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { CustomersService, Customer } from '../customers.service';
import { NotificationService } from '@core/services/notification.service';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-customers-list',
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
    MatTooltipModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Customers"
        [breadcrumbs]="[{ label: 'Sales' }, { label: 'Customers' }]"
        actionLabel="New Customer"
        actionIcon="add"
        (action)="router.navigate(['/admin/customers/new'])"
      />

      <div class="table-toolbar">
        <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchControl" placeholder="Search customers...">
        </mat-form-field>
      </div>

      <div class="table-container mat-elevation-z0">
        <table mat-table [dataSource]="customers()" class="full-width">
          
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef>Name</th>
            <td mat-cell *matCellDef="let customer">
              <div class="customer-info">
                <span class="customer-name">{{ customer.name }}</span>
                <span class="customer-email">{{ customer.email }}</span>
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="phone">
            <th mat-header-cell *matHeaderCellDef>Phone</th>
            <td mat-cell *matCellDef="let customer">{{ customer.phone || 'N/A' }}</td>
          </ng-container>

          <ng-container matColumnDef="city">
            <th mat-header-cell *matHeaderCellDef>City/Country</th>
            <td mat-cell *matCellDef="let customer">
              {{ customer.city }}{{ customer.city && customer.country ? ', ' : '' }}{{ customer.country }}
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-column">Actions</th>
            <td mat-cell *matCellDef="let customer" class="actions-column">
              <button mat-icon-button color="primary" [routerLink]="['/admin/customers', customer.id, 'edit']" matTooltip="Edit Customer" aria-label="Edit customer">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button color="warn" (click)="deleteCustomer(customer)" matTooltip="Delete Customer" aria-label="Delete customer">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
          
          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell empty-cell" [attr.colspan]="columns.length">
              @if (isLoading()) {
                <div class="table-empty-state">
                  <mat-icon>sync</mat-icon>
                  <span class="empty-title">Loading customers...</span>
                </div>
              } @else {
                <div class="table-empty-state">
                  <mat-icon>people</mat-icon>
                  <span class="empty-title">No customers found</span>
                  <span class="empty-subtitle">Try adjusting your search query</span>
                </div>
              }
            </td>
          </tr>
        </table>

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
  `,
  styles: [`
    .table-toolbar {
      display: flex;
      margin-bottom: 16px;
    }

    .search-field {
      width: 100%;
      max-width: 400px;
    }

    .table-container {
      background-color: var(--surface-card);
      border-radius: 12px;
      overflow-x: auto;
      border: 1px solid var(--border-color);
    }

    .full-width {
      width: 100%;
    }

    .customer-info {
      display: flex;
      flex-direction: column;
      padding: 8px 0;
    }

    .customer-name {
      font-weight: 500;
    }

    .customer-email {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }

    .actions-column {
      width: 120px;
      text-align: right;
    }

    .empty-cell {
      text-align: center;
      padding: 48px;
      color: var(--text-secondary);
    }
  `]
})
export class CustomersListComponent implements OnInit {
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['name', 'phone', 'city', 'actions'];
  readonly customers = signal<Customer[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');

  ngOnInit(): void {
    this.loadCustomers();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadCustomers();
      });
  }

  loadCustomers(): void {
    this.isLoading.set(true);
    this.customersService.getCustomers({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined
    }).subscribe({
      next: (result) => {
        this.customers.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load customers');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadCustomers();
  }

  deleteCustomer(customer: Customer): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Customer',
        message: 'Are you sure you want to delete this customer?',
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.customersService.deleteCustomer(customer.id).subscribe({
          next: () => {
            this.notification.success('Customer deleted successfully');
            this.loadCustomers();
          },
          error: () => this.notification.error('Failed to delete customer')
        });
      }
    });
  }
}
