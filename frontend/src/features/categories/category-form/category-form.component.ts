import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { CategoriesService } from '../categories.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        [title]="isEditMode() ? 'Edit Category' : 'New Category'"
        [breadcrumbs]="[
          { label: 'Inventory' },
          { label: 'Categories', link: '/admin/categories' },
          { label: isEditMode() ? 'Edit' : 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="categoryForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Category Name</mat-label>
              <input matInput formControlName="name" placeholder="e.g., Electronics">
              @if (categoryForm.controls.name.hasError('required')) {
                <mat-error>Category name is required</mat-error>
              }
            </mat-form-field>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/categories'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="categoryForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : 'Save Category' }}
              </button>
            </div>
            
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .form-card {
      max-width: 500px;
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
export class CategoryFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly categoriesService = inject(CategoriesService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isEditMode = signal(false);
  readonly isSaving = signal(false);
  categoryId: number | null = null;

  readonly categoryForm = this.fb.nonNullable.group({
    name: ['', Validators.required]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.categoryId = +id;
      this.loadCategory(this.categoryId);
    }
  }

  private loadCategory(id: number): void {
    this.categoriesService.getCategory(id).subscribe({
      next: (category) => {
        this.categoryForm.patchValue(category);
      },
      error: () => {
        this.notification.error('Failed to load category details');
        this.router.navigate(['/admin/categories']);
      }
    });
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) return;

    this.isSaving.set(true);
    const data = this.categoryForm.getRawValue();

    const request$ = this.isEditMode() && this.categoryId
      ? this.categoriesService.updateCategory(this.categoryId, data)
      : this.categoriesService.createCategory(data);

    (request$ as any).subscribe({
      next: () => {
        const msg = 'Category successfully ' + (this.isEditMode() ? 'updated' : 'created');
        this.notification.success(msg);
        this.router.navigate(['/admin/categories']);
      },
      error: () => {
        const msg = 'Failed to ' + (this.isEditMode() ? 'update' : 'create') + ' category';
        this.notification.error(msg);
        this.isSaving.set(false);
      }
    });
  }
}
