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

import { PurchaseOrdersService } from '../purchase-orders.service';
import { PurchaseOrder } from '../models/purchase-order.model';
import { SuppliersService } from '@features/suppliers/suppliers.service';
import { Supplier } from '@features/suppliers/models/supplier.model';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { LocaleCurrencyPipe, LocaleDatePipe } from '@shared/pipes/localized-formatting.pipes';

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
    MatDialogModule,
    PageHeaderComponent,
    StatusBadgeComponent,
    TranslatePipe,
    LocaleCurrencyPipe,
    LocaleDatePipe
  ],
  templateUrl: './purchase-orders-list.component.html',
  styleUrl: './purchase-orders-list.component.scss'
})
export class PurchaseOrdersListComponent implements OnInit {
  private readonly posService = inject(PurchaseOrdersService);
  private readonly suppliersService = inject(SuppliersService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  private readonly i18n = inject(TranslationService);
  readonly router = inject(Router);

  readonly columns = ['id', 'supplier', 'orderDate', 'deliveryDate', 'total', 'status', 'actions'];
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
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }

  submitOrder(order: PurchaseOrder): void {
    this.posService.submitOrder(order.id).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('purchaseOrders.submittedSuccess'));
        this.loadOrders();
      }
    });
  }

  approveOrder(order: PurchaseOrder): void {
    this.posService.approveOrder(order.id).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('purchaseOrders.approvedSuccess'));
        this.loadOrders();
      }
    });
  }

  receiveOrder(order: PurchaseOrder): void {
    this.posService.receiveOrder(order.id).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('purchaseOrders.receivedSuccess'));
        this.loadOrders();
      }
    });
  }

  cancelOrder(order: PurchaseOrder): void {
    this.posService.cancelOrder(order.id).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('purchaseOrders.cancelledSuccess'));
        this.loadOrders();
      }
    });
  }

  deleteOrder(order: PurchaseOrder): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: this.i18n.translate('purchaseOrders.deleteTitle'),
        message: this.i18n.translate('purchaseOrders.deleteMessage'),
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.posService.deletePurchaseOrder(order.id).subscribe({
          next: () => {
            this.notification.success(this.i18n.translate('purchaseOrders.deletedSuccess'));
            this.loadOrders();
          }
        });
      }
    });
  }
}
