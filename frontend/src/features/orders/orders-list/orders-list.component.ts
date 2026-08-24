import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { CurrencyPipe, DatePipe } from '@angular/common';

import { OrdersService } from '../orders.service';
import { Order } from '../models/order.model';
import { InvoicesService } from '@features/invoices/invoices.service';
import { CustomersService } from '@features/customers/customers.service';
import { Customer } from '@features/customers/models/customer.model';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';

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
    MatDialogModule,
    PageHeaderComponent,
    StatusBadgeComponent,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './orders-list.component.html',
  styleUrl: './orders-list.component.scss'
})
export class OrdersListComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly invoicesService = inject(InvoicesService);
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['id', 'customer', 'date', 'total', 'status', 'actions'];
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
      search: this.searchControl.value || undefined,
      customerId: customerId !== null ? customerId : undefined,
      status: status !== null ? status : undefined
    }).subscribe({
      next: (result) => {
        this.orders.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load orders');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }

  updateStatus(order: Order, status: string): void {
    this.ordersService.updateOrderStatus(order.id, status).subscribe({
      next: () => {
        this.notification.success(`Order status updated to ${status}`);
        this.loadOrders();
      },
      error: () => this.notification.error('Failed to update status')
    });
  }

  generateInvoice(order: Order): void {
    const dueDate = new Date();
    dueDate.setDate(dueDate.getDate() + 30); // 30 days default
    
    this.invoicesService.generateFromOrder({
      orderId: order.id,
      dueDate: dueDate.toISOString()
    }).subscribe({
      next: (invoiceId) => {
        this.notification.success(`Invoice INV-${invoiceId} successfully created`);
        this.router.navigate(['/admin/invoices']);
      },
      error: () => this.notification.error('Failed to generate invoice from order')
    });
  }

  deleteOrder(order: Order): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Order',
        message: 'Are you sure you want to delete this order?',
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.ordersService.deleteOrder(order.id).subscribe({
          next: () => {
            this.notification.success('Order deleted successfully');
            this.loadOrders();
          },
          error: () => this.notification.error('Failed to delete order')
        });
      }
    });
  }
}
