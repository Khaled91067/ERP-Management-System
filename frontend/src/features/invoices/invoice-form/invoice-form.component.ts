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
import { CustomersService, Customer } from '@features/customers/customers.service';
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
  template: `
    <div class="page-container">
      <app-page-header
        title="Create Manual Invoice"
        [breadcrumbs]="[
          { label: 'Finance' },
          { label: 'Invoices', link: '/admin/invoices' },
          { label: 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="invoiceForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Customer</mat-label>
                <mat-select formControlName="customerId">
                  @for (customer of availableCustomers(); track customer.id) {
                    <mat-option [value]="customer.id">{{ customer.name }}</mat-option>
                  }
                </mat-select>
                @if (invoiceForm.controls.customerId.hasError('required')) {
                  <mat-error>Customer is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Due Date</mat-label>
                <input matInput [matDatepicker]="picker" formControlName="dueDate">
                <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
                <mat-datepicker #picker></mat-datepicker>
                @if (invoiceForm.controls.dueDate.hasError('required')) {
                  <mat-error>Due date is required</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="lines-section">
              <div class="lines-header">
                <h3>Invoice Lines</h3>
                <button mat-stroked-button color="primary" type="button" (click)="addLine()">
                  <mat-icon>add</mat-icon> Add Line
                </button>
              </div>

              <div formArrayName="lines" class="lines-container">
                @for (line of lines.controls; track i; let i = $index) {
                  <div [formGroupName]="i" class="line-row">
                    
                    <mat-form-field appearance="outline" class="desc-field">
                      <mat-label>Description</mat-label>
                      <input matInput formControlName="description" placeholder="Consulting Services">
                    </mat-form-field>

                    <mat-form-field appearance="outline" class="qty-field">
                      <mat-label>Qty</mat-label>
                      <input matInput type="number" formControlName="quantity" min="1">
                    </mat-form-field>

                    <mat-form-field appearance="outline" class="cost-field">
                      <mat-label>Unit Price</mat-label>
                      <span matTextPrefix>$&nbsp;</span>
                      <input matInput type="number" formControlName="unitPrice" min="0" step="0.01">
                    </mat-form-field>
                    
                    <mat-form-field appearance="outline" class="tax-field">
                      <mat-label>Tax Rate</mat-label>
                      <span matTextSuffix>%</span>
                      <input matInput type="number" formControlName="taxRate" min="0" max="100" step="1">
                    </mat-form-field>
                    
                    <div class="line-total">
                      {{ calculateLineTotal(i) | currency }}
                    </div>

                    <button mat-icon-button color="warn" type="button" (click)="removeLine(i)" [disabled]="lines.length === 1">
                      <mat-icon>delete</mat-icon>
                    </button>
                  </div>
                }
              </div>
              
              <div class="order-total">
                <strong>Total Amount:</strong>
                <span>{{ calculateOrderTotal() | currency }}</span>
              </div>
            </div>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/invoices'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="invoiceForm.invalid || isSaving()">
                {{ isSaving() ? 'Submitting...' : 'Create Invoice' }}
              </button>
            </div>
            
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .form-card {
      max-width: 1000px;
      padding: 24px 16px;
      border: 1px solid var(--border-color);
      border-radius: 12px;
      background-color: var(--surface-card);
    }

    .form-layout {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .form-row {
      display: flex;
      gap: 16px;
    }
    
    @media (max-width: 600px) {
      .form-row {
        flex-direction: column;
      }
    }

    .half-width {
      flex: 1;
    }

    .lines-section {
      margin-top: 24px;
      padding-top: 24px;
      border-top: 1px solid var(--border-color);
    }
    
    .lines-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    
    .lines-header h3 {
      margin: 0;
      font-weight: 500;
    }

    .line-row {
      display: flex;
      gap: 16px;
      align-items: center;
      margin-bottom: 16px;
      background-color: var(--surface-ground);
      padding: 12px;
      border-radius: 8px;
    }
    
    .desc-field {
      flex: 2;
      margin-bottom: -22px;
    }
    
    .qty-field, .cost-field, .tax-field {
      flex: 1;
      margin-bottom: -22px;
    }
    
    .line-total {
      width: 100px;
      text-align: right;
      font-weight: 500;
    }
    
    .order-total {
      display: flex;
      justify-content: flex-end;
      align-items: center;
      gap: 16px;
      padding: 16px 48px 0 0;
      font-size: 1.2rem;
      border-top: 1px dashed var(--border-color);
      margin-top: 16px;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-top: 32px;
    
      flex-wrap: wrap;
    }
  `]
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
