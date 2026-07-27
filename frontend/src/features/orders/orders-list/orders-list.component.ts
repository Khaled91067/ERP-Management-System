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
import { MatDividerModule } from '@angular/material/divider';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { CurrencyPipe, DatePipe } from '@angular/common';

import { OrdersService, Order } from '../orders.service';
import { CustomersService, Customer } from '@features/customers/customers.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-orders-list',
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
    MatDividerModule,
    PageHeaderComponent,
    StatusBadgeComponent,
    CurrencyPipe,
    DatePipe
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Sales Orders"
        [breadcrumbs]="[{ label: 'Sales' }, { label: 'Orders' }]"
        actionLabel="New Order"
        actionIcon="add"
        (action)="router.navigate(['/admin/orders/new'])"
      />

      <div class="table-toolbar">
        <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchControl" placeholder="Search orders...">
        </mat-form-field>
        
        <mat-form-field appearance="outline" class="filter-field" subscriptSizing="dynamic">
          <mat-label>Customer</mat-label>
          <mat-select [formControl]="customerControl">
            <mat-option [value]="null">All Customers</mat-option>
            @for (customer of customers(); track customer.id) {
              <mat-option [value]="customer.id">{{ customer.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" class="filter-field" subscriptSizing="dynamic">
          <mat-label>Status</mat-label>
          <mat-select [formControl]="statusControl">
            <mat-option [value]="null">All Statuses</mat-option>
            <mat-option value="Pending">Pending</mat-option>
            <mat-option value="Processing">Processing</mat-option>
            <mat-option value="Shipped">Shipped</mat-option>
            <mat-option value="Delivered">Delivered</mat-option>
            <mat-option value="Cancelled">Cancelled</mat-option>
          </mat-select>
        </mat-form-field>
      </div>

      <div class="table-container mat-elevation-z0">
        <table mat-table [dataSource]="orders()" class="full-width">
          
          <ng-container matColumnDef="id">
            <th mat-header-cell *matHeaderCellDef>Order #</th>
            <td mat-cell *matCellDef="let order">ORD-{{ order.id.toString().padStart(5, '0') }}</td>
          </ng-container>

          <ng-container matColumnDef="customer">
            <th mat-header-cell *matHeaderCellDef>Customer</th>
            <td mat-cell *matCellDef="let order">
              <div class="customer-info">
                <span class="customer-name">{{ order.customerName }}</span>
                <span class="order-date">{{ order.orderDate | date:'shortDate' }}</span>
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="payment">
            <th mat-header-cell *matHeaderCellDef>Payment</th>
            <td mat-cell *matCellDef="let order">{{ order.paymentMethod }}</td>
          </ng-container>

          <ng-container matColumnDef="total">
            <th mat-header-cell *matHeaderCellDef class="text-right">Total</th>
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
              <button mat-icon-button [matMenuTriggerFor]="menu" aria-label="Order options">
                <mat-icon>more_vert</mat-icon>
              </button>
              <mat-menu #menu="matMenu">
                <button mat-menu-item (click)="updateStatus(order, 'Processing')" [disabled]="order.status !== 'Pending'">
                  <mat-icon>autorenew</mat-icon>
                  <span>Mark Processing</span>
                </button>
                <button mat-menu-item (click)="updateStatus(order, 'Shipped')" [disabled]="order.status !== 'Processing'">
                  <mat-icon>local_shipping</mat-icon>
                  <span>Mark Shipped</span>
                </button>
                <button mat-menu-item (click)="updateStatus(order, 'Delivered')" [disabled]="order.status !== 'Shipped'">
                  <mat-icon>done_all</mat-icon>
                  <span>Mark Delivered</span>
                </button>
                <mat-divider></mat-divider>
                <button mat-menu-item (click)="updateStatus(order, 'Cancelled')" [disabled]="order.status === 'Cancelled' || order.status === 'Delivered'">
                  <mat-icon>cancel</mat-icon>
                  <span>Cancel Order</span>
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
                  <span class="empty-title">Loading orders...</span>
                </div>
              } @else {
                <div class="table-empty-state">
                  <mat-icon>shopping_cart_checkout</mat-icon>
                  <span class="empty-title">No orders found</span>
                  <span class="empty-subtitle">Try adjusting your status or customer filters</span>
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

    .customer-info {
      display: flex;
      flex-direction: column;
      padding: 8px 0;
    }

    .customer-name {
      font-weight: 500;
    }

    .order-date {
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
export class OrdersListComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  readonly router = inject(Router);

  readonly columns = ['id', 'customer', 'payment', 'total', 'status', 'actions'];
  readonly orders = signal<Order[]>([]);
  readonly customers = signal<Customer[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');
  readonly customerControl = new FormControl<number | null>(null);
  readonly statusControl = new FormControl<string | null>(null);

  ngOnInit(): void {
    this.loadCustomers();
    this.loadOrders();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadOrders();
      });
      
    this.customerControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadOrders();
    });
    
    this.statusControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadOrders();
    });
  }
  
  loadCustomers(): void {
    this.customersService.getCustomers({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.customers.set(res.items)
    });
  }

  loadOrders(): void {
    this.isLoading.set(true);
    
    const customerId = this.customerControl.value;
    const status = this.statusControl.value;
    
    this.ordersService.getOrders({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      searchTerm: this.searchControl.value || undefined,
      customerId: customerId !== null ? customerId : undefined,
      status: status !== null ? status : undefined
    }).subscribe({
      next: (result) => {
        this.orders.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load sales orders');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }

  updateStatus(order: Order, newStatus: string): void {
    this.ordersService.updateStatus(order.id, newStatus).subscribe({
      next: () => {
        this.notification.success('Status updated to ' + newStatus);
        this.loadOrders();
      },
      error: () => this.notification.error('Failed to update status')
    });
  }
}
