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
  template: `
    <div class="page-container">
      <app-page-header
        title="Add Department"
        [breadcrumbs]="[
          { label: 'HR' },
          { label: 'Departments', link: '/admin/departments' },
          { label: 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="deptForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Department Name</mat-label>
              <input matInput formControlName="name" placeholder="E.g. Engineering">
              @if (deptForm.controls.name.hasError('required')) {
                <mat-error>Name is required</mat-error>
              }
            </mat-form-field>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/departments'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="deptForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : 'Save Department' }}
              </button>
            </div>
            
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .form-card {
      max-width: 600px;
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
