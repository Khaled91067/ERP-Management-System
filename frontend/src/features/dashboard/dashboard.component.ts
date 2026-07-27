import { Component, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DashboardService, DashboardData } from './dashboard.service';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CurrencyPipe,
    DatePipe,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    StatusBadgeComponent
  ],
  template: `
    <div class="dashboard-container">
      <div class="page-header">
        <h1 class="page-title">Dashboard Overview</h1>
      </div>

      @if (isLoading() && !data()) {
        <div class="loading-state">
          <mat-spinner diameter="48"></mat-spinner>
          <p>Loading dashboard data...</p>
        </div>
      } @else if (hasError()) {
        <div class="error-state">
          <mat-icon class="error-icon">error_outline</mat-icon>
          <h3>Failed to load dashboard data</h3>
          <p>An error occurred while communicating with the server.</p>
          <button type="button" mat-flat-button color="primary" (click)="loadData()">
            <mat-icon>refresh</mat-icon>
            <span>Retry</span>
          </button>
        </div>
      } @else if (data()) {
        <!-- KPI Cards -->
        <div class="kpi-grid">
          <mat-card class="kpi-card">
            <mat-card-content class="kpi-content">
              <div class="kpi-details">
                <span class="kpi-label">Total Revenue</span>
                <span class="kpi-value">{{ data()!.metrics.totalRevenue | currency }}</span>
              </div>
              <div class="kpi-icon-wrapper kpi-revenue">
                <mat-icon>payments</mat-icon>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="kpi-card interactive" [routerLink]="'/admin/orders'" matTooltip="View all orders">
            <mat-card-content class="kpi-content">
              <div class="kpi-details">
                <span class="kpi-label">Total Orders</span>
                <span class="kpi-value">{{ data()!.metrics.totalOrders }}</span>
              </div>
              <div class="kpi-icon-wrapper kpi-orders">
                <mat-icon>shopping_cart</mat-icon>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="kpi-card interactive" [routerLink]="'/admin/products'" matTooltip="View all products">
            <mat-card-content class="kpi-content">
              <div class="kpi-details">
                <span class="kpi-label">Total Products</span>
                <span class="kpi-value">{{ data()!.metrics.totalProducts }}</span>
              </div>
              <div class="kpi-icon-wrapper kpi-products">
                <mat-icon>inventory_2</mat-icon>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="kpi-card interactive" [routerLink]="'/admin/customers'" matTooltip="View all customers">
            <mat-card-content class="kpi-content">
              <div class="kpi-details">
                <span class="kpi-label">Total Customers</span>
                <span class="kpi-value">{{ data()!.metrics.totalCustomers }}</span>
              </div>
              <div class="kpi-icon-wrapper kpi-customers">
                <mat-icon>people</mat-icon>
              </div>
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Data Tables -->
        <div class="tables-grid">
          <!-- Recent Orders -->
          <mat-card class="table-card">
            <div class="table-card-header">
              <span class="table-title">Recent Orders</span>
              <a [routerLink]="'/admin/orders'" class="view-all-link">
                <span>View All</span>
                <mat-icon>arrow_forward</mat-icon>
              </a>
            </div>
            <mat-card-content>
              <div class="table-container mat-elevation-z0">
                <table mat-table [dataSource]="data()!.recentOrders" class="full-width">
                <ng-container matColumnDef="id">
                  <th mat-header-cell *matHeaderCellDef>Order #</th>
                  <td mat-cell *matCellDef="let order">
                    <a [routerLink]="['/admin/orders']" class="row-link">ORD-{{ order.id }}</a>
                  </td>
                </ng-container>
                
                <ng-container matColumnDef="customerName">
                  <th mat-header-cell *matHeaderCellDef>Customer</th>
                  <td mat-cell *matCellDef="let order">{{ order.customerName }}</td>
                </ng-container>

                <ng-container matColumnDef="orderDate">
                  <th mat-header-cell *matHeaderCellDef>Date</th>
                  <td mat-cell *matCellDef="let order">{{ order.orderDate | date:'shortDate' }}</td>
                </ng-container>

                <ng-container matColumnDef="status">
                  <th mat-header-cell *matHeaderCellDef>Status</th>
                  <td mat-cell *matCellDef="let order">
                    <app-status-badge [status]="order.status"></app-status-badge>
                  </td>
                </ng-container>

                <ng-container matColumnDef="totalAmount">
                  <th mat-header-cell *matHeaderCellDef>Total</th>
                  <td mat-cell *matCellDef="let order" class="amount-cell">{{ order.totalAmount | currency }}</td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="recentOrdersColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: recentOrdersColumns;"></tr>
                
                <tr class="mat-row" *matNoDataRow>
                  <td class="mat-cell empty-cell" colspan="5">No recent orders found</td>
                </tr>
              </table>
              </div>
            </mat-card-content>
          </mat-card>

          <!-- Low Stock Alerts -->
          <mat-card class="table-card">
            <div class="table-card-header">
              <span class="table-title">Low Stock Alerts</span>
              <a [routerLink]="'/admin/products'" class="view-all-link">
                <span>View All</span>
                <mat-icon>arrow_forward</mat-icon>
              </a>
            </div>
            <mat-card-content>
              <div class="table-container mat-elevation-z0">
                <table mat-table [dataSource]="data()!.lowStockProducts" class="full-width">
                <ng-container matColumnDef="name">
                  <th mat-header-cell *matHeaderCellDef>Product</th>
                  <td mat-cell *matCellDef="let product">
                    <div class="product-info">
                      <a [routerLink]="['/admin/products']" class="product-name row-link">{{ product.name }}</a>
                      <span class="product-sku">{{ product.sku }}</span>
                    </div>
                  </td>
                </ng-container>
                
                <ng-container matColumnDef="stock">
                  <th mat-header-cell *matHeaderCellDef>Stock</th>
                  <td mat-cell *matCellDef="let product" class="stock-critical">
                    {{ product.stockQuantity }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="reorderLevel">
                  <th mat-header-cell *matHeaderCellDef>Reorder At</th>
                  <td mat-cell *matCellDef="let product">{{ product.reorderLevel }}</td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="lowStockColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: lowStockColumns;"></tr>
                
                <tr class="mat-row" *matNoDataRow>
                  <td class="mat-cell empty-cell" colspan="3">No low stock alerts</td>
                </tr>
              </table>
              </div>
            </mat-card-content>
          </mat-card>
        </div>
      }
    </div>
  `,
  styles: [`
    .dashboard-container {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }

    .page-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      margin-bottom: 8px;

      .header-titles {
        display: flex;
        flex-direction: column;
      }
    }

    .page-title {
      margin: 0 0 4px 0;
      font-size: 1.75rem;
      font-weight: 700;
      color: var(--text-primary);
    }

    .page-subtitle {
      margin: 0;
      color: var(--text-secondary);
      font-size: 0.9375rem;
    }

    .refresh-btn {
      border-radius: var(--radius-button);
      gap: 6px;

      mat-icon.spinning {
        animation: spin 1s linear infinite;
      }
    }

    @keyframes spin {
      100% { transform: rotate(360deg); }
    }

    .loading-state, .error-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 64px 0;
      gap: 16px;
      color: var(--text-secondary);

      .error-icon {
        font-size: 48px;
        width: 48px;
        height: 48px;
        color: var(--status-error-text);
      }

      h3 {
        margin: 0;
        color: var(--text-primary);
      }

      p {
        margin: 0 0 8px 0;
      }
    }

    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(min(100%, 240px), 1fr));
      gap: 24px;
    }

    .kpi-card {
      &.interactive {
        cursor: pointer;
        transition: transform 0.15s ease, box-shadow 0.15s ease, border-color 0.15s ease;

        &:hover {
          transform: translateY(-2px);
          border-color: var(--primary-base);
          box-shadow: var(--shadow-md);
        }
      }
    }

    .kpi-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px !important;
    }

    .kpi-details {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .kpi-label {
      font-size: 0.875rem;
      color: var(--text-secondary);
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .kpi-value {
      font-size: 1.75rem;
      font-weight: 700;
      color: var(--text-primary);
    }

    .kpi-icon-wrapper {
      width: 48px;
      height: 48px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;

      mat-icon {
        font-size: 24px;
        width: 24px;
        height: 24px;
      }
    }

    .kpi-revenue {
      background-color: rgba(198, 106, 69, 0.1);
      color: var(--primary-base);
    }

    .kpi-orders {
      background-color: rgba(99, 102, 241, 0.1);
      color: #6366F1;
    }

    .kpi-products {
      background-color: rgba(245, 158, 11, 0.1);
      color: #F59E0B;
    }

    .kpi-customers {
      background-color: rgba(59, 130, 246, 0.1);
      color: #3B82F6;
    }

    .tables-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 24px;
    }

    @media (max-width: 1024px) {
      .tables-grid {
        grid-template-columns: 1fr;
      }
    }

    .table-card-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 20px 24px 12px;
      border-bottom: 1px solid var(--border-divider);
    }

    .table-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--text-primary);
    }

    .view-all-link {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--primary-base);
      text-decoration: none;
      transition: color 0.15s ease;

      mat-icon {
        font-size: 16px;
        width: 16px;
        height: 16px;
        transition: transform 0.15s ease;
      }

      &:hover {
        color: var(--primary-hover);
        
        mat-icon {
          transform: translateX(2px);
        }
      }
    }

    .row-link {
      color: var(--primary-base);
      text-decoration: none;
      font-weight: 600;

      &:hover {
        text-decoration: underline;
      }
    }

    .amount-cell {
      font-weight: 600;
    }

    .full-width {
      width: 100%;
    }

    .empty-cell {
      text-align: center;
      padding: 32px;
      color: var(--text-secondary);
    }

    .product-info {
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .product-name {
      font-weight: 500;
    }

    .product-sku {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }

    .stock-critical {
      color: var(--status-error-text);
      font-weight: 600;
    }
  `]
})
export class DashboardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  
  readonly data = signal<DashboardData | null>(null);
  readonly isLoading = signal(true);
  readonly hasError = signal(false);

  readonly recentOrdersColumns = ['id', 'customerName', 'orderDate', 'status', 'totalAmount'];
  readonly lowStockColumns = ['name', 'stock', 'reorderLevel'];

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    this.dashboardService.getDashboardData().subscribe({
      next: (result) => {
        this.data.set(result);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to fetch dashboard metrics', err);
        this.hasError.set(true);
        this.isLoading.set(false);
      }
    });
  }
}
