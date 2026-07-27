import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';

import { ProductsService } from '../products.service';
import { CategoriesService, Category } from '@features/categories/categories.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatSelectModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        [title]="isEditMode() ? 'Edit Product' : 'New Product'"
        [breadcrumbs]="[
          { label: 'Inventory' },
          { label: 'Products', link: '/admin/products' },
          { label: isEditMode() ? 'Edit' : 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="productForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Product Name</mat-label>
                <input matInput formControlName="name" placeholder="Wireless Mouse">
                @if (productForm.controls.name.hasError('required')) {
                  <mat-error>Name is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>SKU</mat-label>
                <input matInput formControlName="sku" placeholder="EL-WM-001">
                @if (productForm.controls.sku.hasError('required')) {
                  <mat-error>SKU is required</mat-error>
                }
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Category</mat-label>
              <mat-select formControlName="categoryId">
                @for (category of availableCategories(); track category.id) {
                  <mat-option [value]="category.id">{{ category.name }}</mat-option>
                }
              </mat-select>
              @if (productForm.controls.categoryId.hasError('required')) {
                <mat-error>Category is required</mat-error>
              }
            </mat-form-field>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Unit Price</mat-label>
                <span matTextPrefix>$&nbsp;</span>
                <input matInput type="number" formControlName="unitPrice" placeholder="0.00" min="0" step="0.01">
                @if (productForm.controls.unitPrice.hasError('required')) {
                  <mat-error>Price is required</mat-error>
                }
                @if (productForm.controls.unitPrice.hasError('min')) {
                  <mat-error>Must be >= 0</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Cost Price</mat-label>
                <span matTextPrefix>$&nbsp;</span>
                <input matInput type="number" formControlName="costPrice" placeholder="0.00" min="0" step="0.01">
                @if (productForm.controls.costPrice.hasError('required')) {
                  <mat-error>Cost is required</mat-error>
                }
                @if (productForm.controls.costPrice.hasError('min')) {
                  <mat-error>Must be >= 0</mat-error>
                }
              </mat-form-field>
            </div>
            
            <div class="form-row">
              @if (!isEditMode()) {
                <mat-form-field appearance="outline" class="half-width">
                  <mat-label>Initial Stock Quantity</mat-label>
                  <input matInput type="number" formControlName="initialStockQuantity" placeholder="0" min="0">
                  @if (productForm.controls.initialStockQuantity?.hasError('required')) {
                    <mat-error>Stock is required</mat-error>
                  }
                  @if (productForm.controls.initialStockQuantity?.hasError('min')) {
                    <mat-error>Cannot be negative</mat-error>
                  }
                </mat-form-field>
              }

              <mat-form-field appearance="outline" [class.half-width]="!isEditMode()" [class.full-width]="isEditMode()">
                <mat-label>Reorder Level</mat-label>
                <input matInput type="number" formControlName="reorderLevel" placeholder="10" min="0">
                @if (productForm.controls.reorderLevel.hasError('required')) {
                  <mat-error>Reorder level is required</mat-error>
                }
                @if (productForm.controls.reorderLevel.hasError('min')) {
                  <mat-error>Cannot be negative</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/products'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="productForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : 'Save Product' }}
              </button>
            </div>
            
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .form-card {
      max-width: 800px;
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

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-top: 16px;
    
      flex-wrap: wrap;
    }
  `]
})
export class ProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(ProductsService);
  private readonly categoriesService = inject(CategoriesService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isEditMode = signal(false);
  readonly isSaving = signal(false);
  readonly availableCategories = signal<Category[]>([]);
  productId: number | null = null;

  readonly productForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    sku: ['', Validators.required],
    categoryId: [0 as number | null, Validators.required],
    unitPrice: [0, [Validators.required, Validators.min(0)]],
    costPrice: [0, [Validators.required, Validators.min(0)]],
    initialStockQuantity: [0], // Optional in edit mode
    reorderLevel: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    this.loadCategories();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.productId = +id;
      // In edit mode, initial stock quantity is not part of the update dto
      this.productForm.controls.initialStockQuantity.clearValidators();
      this.productForm.controls.initialStockQuantity.updateValueAndValidity();
      
      this.loadProduct(this.productId);
    } else {
      this.productForm.controls.initialStockQuantity.setValidators([Validators.required, Validators.min(0)]);
      this.productForm.controls.initialStockQuantity.updateValueAndValidity();
    }
  }
  
  private loadCategories(): void {
    this.categoriesService.getCategories({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.availableCategories.set(res.items),
      error: () => this.notification.error('Failed to load categories')
    });
  }

  private loadProduct(id: number): void {
    this.productsService.getProduct(id).subscribe({
      next: (product) => {
        this.productForm.patchValue({
          name: product.name,
          sku: product.sku,
          categoryId: product.categoryId,
          unitPrice: product.unitPrice,
          costPrice: product.costPrice,
          reorderLevel: product.reorderLevel
        });
      },
      error: () => {
        this.notification.error('Failed to load product details');
        this.router.navigate(['/admin/products']);
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) return;

    this.isSaving.set(true);
    const data = this.productForm.getRawValue();

    const request$ = this.isEditMode() && this.productId
      ? this.productsService.updateProduct(this.productId, {
          name: data.name,
          sku: data.sku,
          categoryId: data.categoryId as number,
          unitPrice: data.unitPrice,
          costPrice: data.costPrice,
          reorderLevel: data.reorderLevel
        })
      : this.productsService.createProduct({
          name: data.name,
          sku: data.sku,
          categoryId: data.categoryId as number,
          unitPrice: data.unitPrice,
          costPrice: data.costPrice,
          initialStockQuantity: data.initialStockQuantity as any,
          reorderLevel: data.reorderLevel
        } as any);

    (request$ as any).subscribe({
      next: () => {
        const msg = 'Product successfully ' + (this.isEditMode() ? 'updated' : 'created');
        this.notification.success(msg);
        this.router.navigate(['/admin/products']);
      },
      error: () => {
        const msg = 'Failed to ' + (this.isEditMode() ? 'update' : 'create') + ' product';
        this.notification.error(msg);
        this.isSaving.set(false);
      }
    });
  }
}
