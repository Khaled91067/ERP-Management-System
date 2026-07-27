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
import { SuppliersService, Supplier } from '@features/suppliers/suppliers.service';
import { ProductsService, Product } from '@features/products/products.service';
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
  template: `
    <div class="page-container">
      <app-page-header
        title="Create Purchase Order"
        [breadcrumbs]="[
          { label: 'Purchasing' },
          { label: 'Purchase Orders', link: '/admin/purchase-orders' },
          { label: 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="poForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Supplier</mat-label>
                <mat-select formControlName="supplierId">
                  @for (supplier of availableSuppliers(); track supplier.id) {
                    <mat-option [value]="supplier.id">{{ supplier.companyName }}</mat-option>
                  }
                </mat-select>
                @if (poForm.controls.supplierId.hasError('required')) {
                  <mat-error>Supplier is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Expected Delivery</mat-label>
                <input matInput [matDatepicker]="picker" formControlName="expectedDelivery">
                <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
                <mat-datepicker #picker></mat-datepicker>
                @if (poForm.controls.expectedDelivery.hasError('required')) {
                  <mat-error>Expected delivery date is required</mat-error>
                }
              </mat-form-field>
            </div>

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
                          <mat-option [value]="product.id">{{ product.name }} (SKU: {{ product.sku }})</mat-option>
                        }
                      </mat-select>
                    </mat-form-field>

                    <mat-form-field appearance="outline" class="qty-field">
                      <mat-label>Quantity</mat-label>
                      <input matInput type="number" formControlName="quantity" min="1">
                    </mat-form-field>

                    <mat-form-field appearance="outline" class="cost-field">
                      <mat-label>Unit Cost</mat-label>
                      <span matTextPrefix>$&nbsp;</span>
                      <input matInput type="number" formControlName="unitCost" min="0" step="0.01">
                    </mat-form-field>
                    
                    <div class="line-total">
                      {{ (line.get('quantity')?.value || 0) * (line.get('unitCost')?.value || 0) | currency }}
                    </div>

                    <button mat-icon-button color="warn" type="button" (click)="removeLine(i)" [disabled]="lines.length === 1">
                      <mat-icon>delete</mat-icon>
                    </button>
                  </div>
                }
              </div>
              
              <div class="order-total">
                <strong>Total Amount:</strong>
                <span>{{ calculateTotal() | currency }}</span>
              </div>
            </div>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/purchase-orders'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="poForm.invalid || isSaving()">
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
      margin-bottom: -22px; /* compensate for mat-form-field bottom padding */
    }
    
    .qty-field, .cost-field {
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
    expectedDelivery: [new Date(), Validators.required],
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
      productId: [null as number | null, Validators.required],
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
      const lineGroup = this.lines.at(index);
      // Auto-fill unitCost with the product's costPrice
      lineGroup.patchValue({ unitCost: product.costPrice });
    }
  }

  calculateTotal(): number {
    return this.lines.controls.reduce((total, line) => {
      const qty = line.get('quantity')?.value || 0;
      const cost = line.get('unitCost')?.value || 0;
      return total + (qty * cost);
    }, 0);
  }

  onSubmit(): void {
    if (this.poForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.poForm.getRawValue();
    
    // Format date as ISO string
    const expectedDelivery = new Date(formValue.expectedDelivery).toISOString();

    const requestData = {
      supplierId: formValue.supplierId as number,
      expectedDelivery,
      lines: formValue.lines.map((line: any) => ({
        productId: line.productId as number,
        quantity: line.quantity as number,
        unitCost: line.unitCost as number
      }))
    };

    this.posService.createPurchaseOrder(requestData).subscribe({
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
