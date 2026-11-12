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

import { PurchaseOrdersService } from '../purchase-orders.service';
import { SuppliersService } from '@features/suppliers/suppliers.service';
import { Supplier } from '@features/suppliers/models/supplier.model';
import { ProductsService } from '@features/products/products.service';
import { Product } from '@features/products/models/product.model';
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { LocaleCurrencyPipe, toDateOnlyIsoString } from '@shared/pipes/localized-formatting.pipes';

@Component({
  selector: 'app-purchase-order-form',
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
    TranslatePipe,
    LocaleCurrencyPipe
  ],
  templateUrl: './purchase-order-form.component.html',
  styleUrl: './purchase-order-form.component.scss'
})
export class PurchaseOrderFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly posService = inject(PurchaseOrdersService);
  private readonly suppliersService = inject(SuppliersService);
  private readonly productsService = inject(ProductsService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  private readonly i18n = inject(TranslationService);
  readonly router = inject(Router);

  readonly isSaving = signal(false);
  readonly availableSuppliers = signal<Supplier[]>([]);
  readonly availableProducts = signal<Product[]>([]);

  readonly poForm = this.fb.nonNullable.group({
    supplierId: [null as number | null, Validators.required],
    expectedDeliveryDate: [new Date(Date.now() + 7 * 24 * 60 * 60 * 1000), Validators.required],
    lines: this.fb.array([this.createLineFormGroup()])
  });

  get lines() {
    return this.poForm.get('lines') as FormArray;
  }

  ngOnInit(): void {
    this.loadSuppliers();
    this.loadProducts();
  }

  private loadSuppliers(): void {
    this.suppliersService.getSuppliers({ page: 1, pageSize: 1000 }).subscribe({
      next: (res) => this.availableSuppliers.set(res.items)
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
      unitCost: [0, [Validators.required, Validators.min(0)]]
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
      const line = this.lines.at(index);
      line.patchValue({
        unitCost: product.costPrice ?? product.unitPrice ?? 0
      });
    }
  }

  calculateLineTotal(index: number): number {
    const line = this.lines.at(index);
    const qty = line.get('quantity')?.value || 0;
    const cost = line.get('unitCost')?.value || 0;
    return qty * cost;
  }

  calculateOrderTotal(): number {
    return this.lines.controls.reduce((total, _, i) => total + this.calculateLineTotal(i), 0);
  }

  onSubmit(): void {
    this.formValidation.prepareFormForSubmission(this.poForm);
    if (this.poForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.poForm.getRawValue();

    const requestData = {
      supplierId: formValue.supplierId as number,
      expectedDelivery: toDateOnlyIsoString(formValue.expectedDeliveryDate),
      lines: formValue.lines.map((line: any) => ({
        productId: line.productId as number,
        quantity: line.quantity as number,
        unitCost: line.unitCost as number
      }))
    };

    this.posService.createPurchaseOrder(requestData as any).subscribe({
      next: () => {
        this.notification.success(this.i18n.translate('purchaseOrders.createdSuccess'));
        this.router.navigate(['/admin/purchase-orders']);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }
}
