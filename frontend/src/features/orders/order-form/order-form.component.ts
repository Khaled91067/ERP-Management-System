import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { CurrencyPipe } from '@angular/common';

import { OrdersService } from '../orders.service';
import { CustomersService, Customer } from '@features/customers/customers.service';
import { ProductsService, Product } from '@features/products/products.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatSelectModule,
    MatIconModule,
    PageHeaderComponent,
    CurrencyPipe
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Create Sales Order"
        [breadcrumbs]="[
          { label: 'Sales' },
          { label: 'Orders', link: '/admin/orders' },
          { label: 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="orderForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Customer</mat-label>
                <mat-select formControlName="customerId" (selectionChange)="onCustomerSelected($event.value)">
                  @for (customer of availableCustomers(); track customer.id) {
                    <mat-option [value]="customer.id">{{ customer.name }}</mat-option>
                  }
                </mat-select>
                @if (orderForm.controls.customerId.hasError('required')) {
                  <mat-error>Customer is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Payment Method</mat-label>
                <mat-select formControlName="paymentMethod">
                  <mat-option value="Cash">Cash</mat-option>
                  <mat-option value="CreditCard">Credit Card</mat-option>
                  <mat-option value="MobilePayment">Mobile Payment</mat-option>
                </mat-select>
                @if (orderForm.controls.paymentMethod.hasError('required')) {
                  <mat-error>Payment method is required</mat-error>
                }
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Shipping Address</mat-label>
              <textarea matInput formControlName="shippingAddress" rows="2" placeholder="123 Main St, City, Country..."></textarea>
              @if (orderForm.controls.shippingAddress.hasError('required')) {
                <mat-error>Shipping address is required</mat-error>
              }
            </mat-form-field>

            <div class="lines-section">
              <div class="lines-header">
                <h3>Order Lines</h3>
                <button mat-stroked-button color="primary" type="button" (click)="addLine()">
                  <mat-icon>add</mat-icon> Add Line
                </button>
              </div>

              <div formArrayName="lines" class="lines-container">
                @for (line of lines.controls; track i; let i = $index) {
                  <div [formGroupName]="i" class="line-row">
                    
                    <mat-form-field appearance="outline" class="product-field">
                      <mat-label>Product</mat-label>
                      <mat-select formControlName="productId" (selectionChange)="onProductSelected(i, $event.value)">
                        @for (product of availableProducts(); track product.id) {
                          <mat-option [value]="product.id">{{ product.name }} (Stock: {{ product.stockQuantity }})</mat-option>
                        }
                      </mat-select>
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
                    
                    <mat-form-field appearance="outline" class="discount-field">
                      <mat-label>Discount</mat-label>
                      <span matTextSuffix>%</span>
                      <input matInput type="number" formControlName="discountPercentage" min="0" max="100" step="1">
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
              <button mat-button type="button" (click)="router.navigate(['/admin/orders'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="orderForm.invalid || isSaving()">
                {{ isSaving() ? 'Submitting...' : 'Submit Order' }}
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

    .full-width {
      width: 100%;
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
    
    .product-field {
      flex: 2;
      margin-bottom: -22px;
    }
    
    .qty-field, .cost-field, .discount-field {
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
export class OrderFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly ordersService = inject(OrdersService);
  private readonly customersService = inject(CustomersService);
  private readonly productsService = inject(ProductsService);
  private readonly notification = inject(NotificationService);
  readonly router = inject(Router);

  readonly isSaving = signal(false);
  readonly availableCustomers = signal<Customer[]>([]);
  readonly availableProducts = signal<Product[]>([]);

  readonly orderForm = this.fb.nonNullable.group({
    customerId: [0 as number | null, Validators.required],
    paymentMethod: ['Cash', Validators.required],
    shippingAddress: ['', Validators.required],
    lines: this.fb.array([this.createLineFormGroup()])
  });

  get lines() {
    return this.orderForm.get('lines') as FormArray;
  }

  ngOnInit(): void {
    this.loadCustomers();
    this.loadProducts();
  }
  
  private loadCustomers(): void {
    this.customersService.getCustomers({ page: 1, pageSize: 1000 }).subscribe({
      next: (res) => this.availableCustomers.set(res.items),
      error: () => this.notification.error('Failed to load customers')
    });
  }
  
  private loadProducts(): void {
    this.productsService.getProducts({ page: 1, pageSize: 1000 }).subscribe({
      next: (res) => this.availableProducts.set(res.items),
      error: () => this.notification.error('Failed to load products')
    });
  }

  onCustomerSelected(customerId: number): void {
    const customer = this.availableCustomers().find(c => c.id === customerId);
    if (customer && !this.orderForm.get('shippingAddress')?.value) {
      // Auto-fill shipping address with customer city/country if blank
      this.orderForm.patchValue({
        shippingAddress: `${customer.city}, ${customer.country}`
      });
    }
  }

  private createLineFormGroup(): FormGroup {
    return this.fb.group({
      productId: [null as number | null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      discountPercentage: [0, [Validators.min(0), Validators.max(100)]]
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

  onProductSelected(index: number, productId: number): void {
    const product = this.availableProducts().find(p => p.id === productId);
    if (product) {
      const lineGroup = this.lines.at(index);
      // Auto-fill unitPrice with the product's selling price
      lineGroup.patchValue({ unitPrice: product.unitPrice });
    }
  }

  calculateLineTotal(index: number): number {
    const line = this.lines.at(index);
    const qty = line.get('quantity')?.value || 0;
    const price = line.get('unitPrice')?.value || 0;
    const discount = line.get('discountPercentage')?.value || 0;
    
    const subtotal = qty * price;
    return subtotal - (subtotal * (discount / 100));
  }

  calculateOrderTotal(): number {
    return this.lines.controls.reduce((total, _, i) => total + this.calculateLineTotal(i), 0);
  }

  onSubmit(): void {
    if (this.orderForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.orderForm.getRawValue();

    const requestData = {
      customerId: formValue.customerId as number,
      paymentMethod: formValue.paymentMethod,
      shippingAddress: formValue.shippingAddress,
      lines: formValue.lines.map((line: any) => ({
        productId: line.productId as number,
        quantity: line.quantity as number,
        unitPrice: line.unitPrice as number,
        discountPercentage: line.discountPercentage as number
      }))
    };

    this.ordersService.createOrder(requestData as any).subscribe({
      next: () => {
        this.notification.success('Order created successfully');
        this.router.navigate(['/admin/orders']);
      },
      error: () => {
        this.notification.error('Failed to create order');
        this.isSaving.set(false);
      }
    });
  }
}
