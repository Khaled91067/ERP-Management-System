import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { CurrencyPipe, DatePipe } from '@angular/common';

import { PurchaseOrdersService, PurchaseOrder } from '../purchase-orders.service';
import { SuppliersService, Supplier } from '@features/suppliers/suppliers.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-purchase-orders-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatMenuModule,
    PageHeaderComponent,
    StatusBadgeComponent,
    CurrencyPipe,
    DatePipe
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Purchase Orders"
        [breadcrumbs]="[{ label: 'Purchasing' }, { label: 'Purchase Orders' }]"
        actionLabel="New PO"
        actionIcon="add"
        (action)="router.navigate(['/admin/purchase-orders/new'])"
      />

      <div class="table-toolbar">
        <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchControl" placeholder="Search POs...">
        </mat-form-field>
        
        <mat-form-field appearance="outline" class="filter-field" subscriptSizing="dynamic">
          <mat-label>Supplier</mat-label>
          <mat-select [formControl]="supplierControl">
            <mat-option [value]="null">All Suppliers</mat-option>
            @for (supplier of suppliers(); track supplier.id) {
              <mat-option [value]="supplier.id">{{ supplier.companyName }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" class="filter-field" subscriptSizing="dynamic">
          <mat-label>Status</mat-label>
          <mat-select [formControl]="statusControl">
            <mat-option [value]="null">All Statuses</mat-option>
            <mat-option value="Pending">Pending</mat-option>
            <mat-option value="Approved">Approved</mat-option>
            <mat-option value="Received">Received</mat-option>
            <mat-option value="Cancelled">Cancelled</mat-option>
          </mat-select>
        </mat-form-field>
      </div>

      <div class="table-container mat-elevation-z0">
        <table mat-table [dataSource]="orders()" class="full-width">
          
          <ng-container matColumnDef="id">
            <th mat-header-cell *matHeaderCellDef>PO #</th>
            <td mat-cell *matCellDef="let order">PO-{{ order.id.toString().padStart(5, '0') }}</td>
          </ng-container>

          <ng-container matColumnDef="dates">
            <th mat-header-cell *matHeaderCellDef>Dates</th>
            <td mat-cell *matCellDef="let order">
              <div class="date-info">
                <span>Ordered: {{ order.orderDate | date:'shortDate' }}</span>
                <span class="text-secondary">Expected: {{ order.expectedDelivery | date:'shortDate' }}</span>
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="total">
            <th mat-header-cell *matHeaderCellDef class="text-right">Total Amount</th>
            <td mat-cell *matCellDef="let order" class="text-right">{{ order.totalAmount | currency }}</td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let order">
              <app-status-badge [status]="order.status"></app-status-badge>
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-column">Actions</th>
            <td mat-cell *matCellDef="let order" class="actions-column">
              <button mat-icon-button [matMenuTriggerFor]="menu" aria-label="Purchase order options">
                <mat-icon>more_vert</mat-icon>
              </button>
              <mat-menu #menu="matMenu">
                <button mat-menu-item (click)="updateStatus(order, 'Approved')" [disabled]="order.status !== 'Pending'">
                  <mat-icon>check_circle</mat-icon>
                  <span>Mark Approved</span>
                </button>
                <button mat-menu-item (click)="updateStatus(order, 'Received')" [disabled]="order.status !== 'Approved'">
                  <mat-icon>inventory_2</mat-icon>
                  <span>Mark Received</span>
                </button>
                <button mat-menu-item (click)="updateStatus(order, 'Cancelled')" [disabled]="order.status === 'Cancelled' || order.status === 'Received'">
                  <mat-icon>cancel</mat-icon>
                  <span>Cancel PO</span>
                </button>
              </mat-menu>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
          
          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell empty-cell" [attr.colspan]="columns.length">
              @if (isLoading()) {
                <div class="table-empty-state">
                  <mat-icon>sync</mat-icon>
                  <span class="empty-title">Loading purchase orders...</span>
                </div>
              } @else {
                <div class="table-empty-state">
                  <mat-icon>receipt</mat-icon>
                  <span class="empty-title">No purchase orders found</span>
                  <span class="empty-subtitle">Try adjusting your status or supplier filters</span>
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
      gap: 16px;
      margin-bottom: 16px;
    }
    
    @media (max-width: 768px) {
      .table-toolbar {
        flex-direction: column;
      }
    }

    .search-field {
      flex: 1;
      max-width: 400px;
    }
    
    .filter-field {
      width: 200px;
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

    .date-info {
      display: flex;
      flex-direction: column;
      padding: 8px 0;
    }

    .text-secondary {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }

    .actions-column {
      width: 80px;
      text-align: right;
    }

    .empty-cell {
      text-align: center;
      padding: 48px;
      color: var(--text-secondary);
    }
  `]
})
export class PurchaseOrdersListComponent implements OnInit {
  private readonly posService = inject(PurchaseOrdersService);
  private readonly suppliersService = inject(SuppliersService);
  private readonly notification = inject(NotificationService);
  readonly router = inject(Router);

  readonly columns = ['id', 'dates', 'total', 'status', 'actions'];
  readonly orders = signal<PurchaseOrder[]>([]);
  readonly suppliers = signal<Supplier[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');
  readonly supplierControl = new FormControl<number | null>(null);
  readonly statusControl = new FormControl<string | null>(null);

  ngOnInit(): void {
    this.loadSuppliers();
    this.loadOrders();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadOrders();
      });
      
    this.supplierControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadOrders();
    });
    
    this.statusControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadOrders();
    });
  }
  
  loadSuppliers(): void {
    this.suppliersService.getSuppliers({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.suppliers.set(res.items)
    });
  }

  loadOrders(): void {
    this.isLoading.set(true);
    
    const supplierId = this.supplierControl.value;
    const status = this.statusControl.value;
    
    this.posService.getPurchaseOrders({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined,
      supplierId: supplierId !== null ? supplierId : undefined,
      status: status !== null ? status : undefined
    }).subscribe({
      next: (result) => {
        this.orders.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load purchase orders');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }

  updateStatus(order: PurchaseOrder, newStatus: string): void {
    this.posService.updateStatus(order.id, newStatus).subscribe({
      next: () => {
        this.notification.success('Status updated to ' + newStatus);
        this.loadOrders();
      },
      error: () => this.notification.error('Failed to update status')
    });
  }
}
