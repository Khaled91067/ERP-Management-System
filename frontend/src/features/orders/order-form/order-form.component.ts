import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';

import { OrdersService } from '../orders.service';
import { CustomersService } from '@features/customers/customers.service';
import { Customer } from '@features/customers/models/customer.model';
import { ProductsService } from '@features/products/products.service';
import { Product } from '@features/products/models/product.model';
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { LocaleCurrencyPipe } from '@shared/pipes/localized-formatting.pipes';

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
    TranslatePipe,
    LocaleCurrencyPipe
  ],
  templateUrl: './order-form.component.html',
  styleUrl: './order-form.component.scss'
})
export class OrderFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly ordersService = inject(OrdersService);
  private readonly customersService = inject(CustomersService);
  private readonly productsService = inject(ProductsService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  private readonly i18n = inject(TranslationService);
  readonly router = inject(Router);

  readonly isSaving = signal(false);
  readonly availableCustomers = signal<Customer[]>([]);
  readonly availableProducts = signal<Product[]>([]);

  readonly orderForm = this.fb.nonNullable.group({
    customerId: [null as number | null, Validators.required],
    paymentMethod: ['Cash', Validators.required],
    shippingAddress: ['', [Validators.required, Validators.maxLength(500)]],
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
      next: (res) => this.availableCustomers.set(res.items)
    });
  }

  private loadProducts(): void {
    this.productsService.getProducts({ page: 1, pageSize: 1000 }).subscribe({
      next: (res) => this.availableProducts.set(res.items)
    });
  }

  private createLineFormGroup(): FormGroup {
    return this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
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

  onCustomerSelected(customerId: number): void {
    const customer = this.availableCustomers().find(c => c.id === customerId);
    if (customer && !this.orderForm.controls.shippingAddress.value) {
      const addressParts = [customer.address, customer.city, customer.country].filter(Boolean);
      if (addressParts.length > 0) {
        this.orderForm.patchValue({
          shippingAddress: addressParts.join(', ')
        });
      }
    }
  }

  onProductSelected(index: number, productId: number): void {
    const product = this.availableProducts().find(p => p.id === productId);
    if (product) {
      const line = this.lines.at(index);
      line.patchValue({
        unitPrice: product.unitPrice
      });
    }
  }

  calculateLineTotal(index: number): number {
    const line = this.lines.at(index);
    const qty = line.get('quantity')?.value || 0;
    const price = line.get('unitPrice')?.value || 0;
    return qty * price;
  }

  calculateOrderTotal(): number {
    return this.lines.controls.reduce((total, _, i) => total + this.calculateLineTotal(i), 0);
  }

  onSubmit(): void {
    this.formValidation.prepareFormForSubmission(this.orderForm);
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
        discountPercentage: 0
      }))
    };

    this.ordersService.createOrder(requestData as any).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('orders.createdSuccess'));
        this.router.navigate(['/admin/orders']);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }
}
