import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
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
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss'
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

    const request$: Observable<unknown> = this.isEditMode() && this.categoryId
      ? this.categoriesService.updateCategory(this.categoryId, data)
      : this.categoriesService.createCategory(data);

    request$.subscribe({
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
