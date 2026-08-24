import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { DepartmentsService } from '../departments.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-department-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    PageHeaderComponent
  ],
  templateUrl: './department-form.component.html',
  styleUrl: './department-form.component.scss'
})
export class DepartmentFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly departmentsService = inject(DepartmentsService);
  private readonly notification = inject(NotificationService);
  readonly router = inject(Router);

  readonly isSaving = signal(false);

  readonly deptForm = this.fb.nonNullable.group({
    name: ['', Validators.required]
  });

  onSubmit(): void {
    if (this.deptForm.invalid) return;

    this.isSaving.set(true);
    
    this.departmentsService.createDepartment(this.deptForm.getRawValue()).subscribe({
      next: () => {
        this.notification.success('Department created successfully');
        this.router.navigate(['/admin/departments']);
      },
      error: () => {
        this.notification.error('Failed to create department');
        this.isSaving.set(false);
      }
    });
  }
}
