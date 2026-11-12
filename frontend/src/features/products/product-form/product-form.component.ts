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
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';

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
    PageHeaderComponent,
    TranslatePipe
  ],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})
export class ProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(ProductsService);
  private readonly categoriesService = inject(CategoriesService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  private readonly i18n = inject(TranslationService);
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
    categoryId: [null as number | null, Validators.required],
    unitPrice: [0, [Validators.required, Validators.min(0)]],
    costPrice: [0, [Validators.required, Validators.min(0)]],
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
      next: (res) => this.availableCategories.set(res.items)
    });
  }

  private loadProduct(id: number): void {
    this.productsService.getProduct(id).subscribe({
      next: (product) => {
        this.productForm.patchValue(product);
      },
      error: () => {
        this.router.navigate(['/admin/products']);
      }
    });
  }

  onSubmit(): void {
    this.formValidation.prepareFormForSubmission(this.productForm);
    if (this.productForm.invalid) return;

    this.isSaving.set(true);
    const formVal = this.productForm.getRawValue();

    const request$: Observable<unknown> = this.isEditMode() && this.productId
      ? this.productsService.updateProduct(this.productId, {
          id: this.productId,
          name: formVal.name,
          sku: formVal.sku,
          categoryId: formVal.categoryId as number,
          unitPrice: formVal.unitPrice,
          costPrice: formVal.costPrice,
          reorderLevel: formVal.reorderLevel
        })
      : this.productsService.createProduct({
          name: formVal.name,
          sku: formVal.sku,
          categoryId: formVal.categoryId as number,
          unitPrice: formVal.unitPrice,
          costPrice: formVal.costPrice,
          initialStockQuantity: formVal.stockQuantity,
          reorderLevel: formVal.reorderLevel
        });

    request$.subscribe({
      next: () => {
        const msgKey = this.isEditMode() ? 'products.updatedSuccess' : 'products.createdSuccess';
        this.notification.success(this.i18n.translate(msgKey));
        this.router.navigate(['/admin/products']);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }
}
