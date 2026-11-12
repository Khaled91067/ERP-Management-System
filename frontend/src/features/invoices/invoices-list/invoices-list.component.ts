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
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { InvoicesService } from '../invoices.service';
import { Invoice } from '../models/invoice.model';
import { CustomersService } from '@features/customers/customers.service';
import { Customer } from '@features/customers/models/customer.model';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { LocaleCurrencyPipe, LocaleDatePipe, parseDateSafe } from '@shared/pipes/localized-formatting.pipes';

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
    TranslatePipe,
    LocaleCurrencyPipe,
    LocaleDatePipe
  ],
  templateUrl: './invoices-list.component.html',
  styleUrl: './invoices-list.component.scss'
})
export class InvoicesListComponent implements OnInit {
  private readonly invoicesService = inject(InvoicesService);
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  private readonly i18n = inject(TranslationService);
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
    const due = parseDateSafe(invoice.dueDate);
    return due ? due < new Date() : false;
  }

  sendInvoice(invoice: Invoice): void {
    this.invoicesService.sendInvoice(invoice.id).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('invoices.sentSuccess'));
        this.loadInvoices();
      }
    });
  }

  payInvoice(invoice: Invoice): void {
    this.invoicesService.payInvoice(invoice.id).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('invoices.paidSuccess'));
        this.loadInvoices();
      }
    });
  }

  deleteInvoice(invoice: Invoice): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: this.i18n.translate('invoices.deleteTitle'),
        message: this.i18n.translate('invoices.deleteMessage'),
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.invoicesService.deleteInvoice(invoice.id).subscribe({
          next: () => {
            this.notification.success(this.i18n.translate('invoices.deletedSuccess'));
            this.loadInvoices();
          }
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
        this.notification.success(this.i18n.translate('invoices.pdfDownloadedSuccess'));
      }
    });
  }
}
