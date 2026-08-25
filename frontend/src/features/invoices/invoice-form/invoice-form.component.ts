import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { CurrencyPipe } from '@angular/common';

import { InvoicesService } from '../invoices.service';
import { CustomersService } from '@features/customers/customers.service';
import { Customer } from '@features/customers/models/customer.model';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    PageHeaderComponent,
    CurrencyPipe
  ],
  templateUrl: './invoice-form.component.html',
  styleUrl: './invoice-form.component.scss'
})
export class InvoiceFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly invoicesService = inject(InvoicesService);
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  readonly router = inject(Router);

  readonly isSaving = signal(false);
  readonly availableCustomers = signal<Customer[]>([]);

  readonly invoiceForm = this.fb.nonNullable.group({
    customerId: [0 as number | null, Validators.required],
    dueDate: [new Date(Date.now() + 30 * 24 * 60 * 60 * 1000), Validators.required], // Default to +30 days
    lines: this.fb.array([this.createLineFormGroup()])
  });

  get lines() {
    return this.invoiceForm.get('lines') as FormArray;
  }

  ngOnInit(): void {
    this.loadCustomers();
  }
  
  private loadCustomers(): void {
    this.customersService.getCustomers({ page: 1, pageSize: 1000 }).subscribe({
      next: (res) => this.availableCustomers.set(res.items),
      error: () => this.notification.error('Failed to load customers')
    });
  }

  private createLineFormGroup(): FormGroup {
    return this.fb.group({
      description: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      taxRate: [0, [Validators.min(0), Validators.max(100)]]
    });
  }

  addLine(): void {
    this.lines.push(this.createLineFormGroup());
  }

  removeLine(index: number): void {
    if (this.lines.length > 1) {
      this.lines.removeAt(index);
    }
  }

  calculateLineTotal(index: number): number {
    const line = this.lines.at(index);
    const qty = line.get('quantity')?.value || 0;
    const price = line.get('unitPrice')?.value || 0;
    const tax = line.get('taxRate')?.value || 0;
    
    const subtotal = qty * price;
    return subtotal + (subtotal * (tax / 100));
  }

  calculateOrderTotal(): number {
    return this.lines.controls.reduce((total, _, i) => total + this.calculateLineTotal(i), 0);
  }

  onSubmit(): void {
    if (this.invoiceForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.invoiceForm.getRawValue();
    
    // Format date as ISO string
    const dueDate = new Date(formValue.dueDate).toISOString();

    const requestData = {
      customerId: formValue.customerId as number,
      dueDate,
      lines: formValue.lines.map((line: any) => ({
        description: line.description as string,
        quantity: line.quantity as number,
        unitPrice: line.unitPrice as number,
        taxRate: line.taxRate as number
      }))
    };

    this.invoicesService.createInvoice(requestData as any).subscribe({
      next: () => {
        this.notification.success('Invoice created successfully');
        this.router.navigate(['/admin/invoices']);
      },
      error: () => {
        this.notification.error('Failed to create invoice');
        this.isSaving.set(false);
      }
    });
  }
}
