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
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { CurrencyPipe, DatePipe } from '@angular/common';

import { InvoicesService, Invoice } from '../invoices.service';
import { CustomersService, Customer } from '@features/customers/customers.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-invoices-list',
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
    MatDialogModule,
    MatTooltipModule,
    PageHeaderComponent,
    StatusBadgeComponent,
    CurrencyPipe,
    DatePipe
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Invoices"
        [breadcrumbs]="[{ label: 'Finance' }, { label: 'Invoices' }]"
        actionLabel="New Invoice"
        actionIcon="add"
        (action)="router.navigate(['/admin/invoices/new'])"
      />

      <div class="table-toolbar">
        <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchControl" placeholder="Search invoices...">
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
            <mat-option value="Paid">Paid</mat-option>
            <mat-option value="Overdue">Overdue</mat-option>
            <mat-option value="Cancelled">Cancelled</mat-option>
          </mat-select>
        </mat-form-field>
      </div>

      <div class="table-container mat-elevation-z0">
        <table mat-table [dataSource]="invoices()" class="full-width">
          
          <ng-container matColumnDef="id">
            <th mat-header-cell *matHeaderCellDef>Invoice #</th>
            <td mat-cell *matCellDef="let invoice">
              <div class="invoice-ids">
                <span class="invoice-number">INV-{{ invoice.id.toString().padStart(5, '0') }}</span>
                @if (invoice.orderId) {
                  <span class="order-ref">Order: ORD-{{ invoice.orderId.toString().padStart(5, '0') }}</span>
                }
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="customer">
            <th mat-header-cell *matHeaderCellDef>Customer</th>
            <td mat-cell *matCellDef="let invoice">{{ invoice.customerName }}</td>
          </ng-container>

          <ng-container matColumnDef="dates">
            <th mat-header-cell *matHeaderCellDef>Dates</th>
            <td mat-cell *matCellDef="let invoice">
              <div class="date-info">
                <span>Date: {{ invoice.invoiceDate | date:'shortDate' }}</span>
                <span class="text-secondary" [class.text-warn]="isOverdue(invoice)">
                  Due: {{ invoice.dueDate | date:'shortDate' }}
                </span>
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="total">
            <th mat-header-cell *matHeaderCellDef class="text-right">Amount</th>
            <td mat-cell *matCellDef="let invoice" class="text-right">{{ invoice.totalAmount | currency }}</td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let invoice">
              <app-status-badge [status]="isOverdue(invoice) ? 'Overdue' : invoice.status"></app-status-badge>
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-column">Actions</th>
            <td mat-cell *matCellDef="let invoice" class="actions-column">
              <button mat-icon-button [matMenuTriggerFor]="menu" aria-label="Invoice options">
                <mat-icon>more_vert</mat-icon>
              </button>
              <mat-menu #menu="matMenu">
                <button mat-menu-item (click)="payInvoice(invoice)" [disabled]="invoice.status === 'Paid' || invoice.status === 'Cancelled'">
                  <mat-icon>payments</mat-icon>
                  <span>Mark as Paid</span>
                </button>
                <button mat-menu-item (click)="downloadPdf(invoice)">
                  <mat-icon>picture_as_pdf</mat-icon>
                  <span>Download PDF</span>
                </button>
                <button mat-menu-item (click)="deleteInvoice(invoice)" [disabled]="invoice.status === 'Paid'">
                  <mat-icon>delete</mat-icon>
                  <span>Delete Invoice</span>
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
                  <span class="empty-title">Loading invoices...</span>
                </div>
              } @else {
                <div class="table-empty-state">
                  <mat-icon>receipt_long</mat-icon>
                  <span class="empty-title">No invoices found</span>
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
      flex-wrap: wrap;
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

    .invoice-ids, .date-info {
      display: flex;
      flex-direction: column;
      padding: 8px 0;
    }

    .invoice-number {
      font-weight: 500;
    }

    .order-ref {
      font-size: 0.75rem;
      color: var(--primary-color);
    }

    .text-secondary {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }
    
    .text-warn {
      color: var(--error-color);
      font-weight: 500;
    }

    .actions-column {
      width: 80px;
      text-align: right;
    }

    .empty-cell {
      text-align: center;
      padding: 24px;
    }
  `]
})
export class InvoicesListComponent implements OnInit {
  private readonly invoicesService = inject(InvoicesService);
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['id', 'customer', 'dates', 'total', 'status', 'actions'];
  readonly invoices = signal<Invoice[]>([]);
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
    this.loadInvoices();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadInvoices();
      });
      
    this.customerControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadInvoices();
    });
    
    this.statusControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadInvoices();
    });
  }
  
  loadCustomers(): void {
    this.customersService.getCustomers({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.customers.set(res.items)
    });
  }

  loadInvoices(): void {
    this.isLoading.set(true);
    
    const customerId = this.customerControl.value;
    const status = this.statusControl.value;
    
    this.invoicesService.getInvoices({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined,
      customerId: customerId !== null ? customerId : undefined,
      status: status !== null ? status : undefined
    }).subscribe({
      next: (result) => {
        this.invoices.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load invoices');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadInvoices();
  }

  isOverdue(invoice: Invoice): boolean {
    if (invoice.status === 'Paid' || invoice.status === 'Cancelled') return false;
    return new Date(invoice.dueDate) < new Date();
  }

  payInvoice(invoice: Invoice): void {
    this.invoicesService.payInvoice(invoice.id).subscribe({
      next: () => {
        this.notification.success('Invoice marked as paid');
        this.loadInvoices();
      },
      error: () => this.notification.error('Failed to update invoice status')
    });
  }

  deleteInvoice(invoice: Invoice): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Invoice',
        message: 'Are you sure you want to delete this invoice?',
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.invoicesService.deleteInvoice(invoice.id).subscribe({
          next: () => {
            this.notification.success('Invoice deleted successfully');
            this.loadInvoices();
          },
          error: () => this.notification.error('Failed to delete invoice')
        });
      }
    });
  }

  downloadPdf(invoice: Invoice): void {
    this.invoicesService.downloadInvoicePdf(invoice.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `INV-${invoice.id.toString().padStart(5, '0')}.pdf`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
        this.notification.success('PDF downloaded successfully');
      },
      error: () => this.notification.error('Failed to download PDF')
    });
  }
}
