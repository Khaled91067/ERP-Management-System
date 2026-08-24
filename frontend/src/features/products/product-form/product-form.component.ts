import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';

import { ProductsService } from '../products.service';
import { CategoriesService } from '@features/categories/categories.service';
import { Category } from '@features/categories/models/category.model';
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
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
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
    description: [''],
    categoryId: [0 as number | null, Validators.required],
    unitPrice: [0, [Validators.required, Validators.min(0)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
    reorderLevel: [10, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    this.loadCategories();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.productId = +id;
      this.loadProduct(this.productId);
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
        this.productForm.patchValue(product);
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
    const formVal = this.productForm.getRawValue();
    const data = {
      ...formVal,
      categoryId: formVal.categoryId as number
    };

    const request$: Observable<unknown> = this.isEditMode() && this.productId
      ? this.productsService.updateProduct(this.productId, { id: this.productId, ...data })
      : this.productsService.createProduct(data);

    request$.subscribe({
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
