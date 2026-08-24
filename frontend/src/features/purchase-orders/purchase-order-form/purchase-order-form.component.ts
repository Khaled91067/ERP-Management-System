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

import { PurchaseOrdersService } from '../purchase-orders.service';
import { SuppliersService } from '@features/suppliers/suppliers.service';
import { Supplier } from '@features/suppliers/models/supplier.model';
import { ProductsService } from '@features/products/products.service';
import { Product } from '@features/products/models/product.model';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

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
    CurrencyPipe
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
  readonly router = inject(Router);

  readonly isSaving = signal(false);
  readonly availableSuppliers = signal<Supplier[]>([]);
  readonly availableProducts = signal<Product[]>([]);

  readonly poForm = this.fb.nonNullable.group({
    supplierId: [0 as number | null, Validators.required],
    expectedDeliveryDate: [new Date(Date.now() + 7 * 24 * 60 * 60 * 1000), Validators.required], // Default +7 days
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
      next: (res) => this.availableSuppliers.set(res.items),
      error: () => this.notification.error('Failed to load suppliers')
    });
  }

  private loadProducts(): void {
    this.productsService.getProducts({ page: 1, pageSize: 1000 }).subscribe({
      next: (res) => this.availableProducts.set(res.items),
      error: () => this.notification.error('Failed to load products')
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
    if (this.poForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.poForm.getRawValue();
    
    const requestData = {
      supplierId: formValue.supplierId as number,
      expectedDeliveryDate: new Date(formValue.expectedDeliveryDate).toISOString(),
      lines: formValue.lines.map((line: any) => ({
        productId: line.productId as number,
        quantity: line.quantity as number,
        unitCost: line.unitCost as number
      }))
    };

    this.posService.createPurchaseOrder(requestData as any).subscribe({
      next: () => {
        this.notification.success('Purchase order created successfully');
        this.router.navigate(['/admin/purchase-orders']);
      },
      error: () => {
        this.notification.error('Failed to create purchase order');
        this.isSaving.set(false);
      }
    });
  }
}
