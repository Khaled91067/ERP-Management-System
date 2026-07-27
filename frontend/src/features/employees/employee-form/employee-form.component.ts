import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { EmployeesService } from '../employees.service';
import { DepartmentsService, Department } from '@features/departments/departments.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-employee-form',
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
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        [title]="isEditMode() ? 'Edit Employee' : 'Add Employee'"
        [breadcrumbs]="[
          { label: 'HR' },
          { label: 'Employees', link: '/admin/employees' },
          { label: isEditMode() ? 'Edit' : 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="empForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>First Name</mat-label>
                <input matInput formControlName="firstName" placeholder="John">
                @if (empForm.controls.firstName.hasError('required')) {
                  <mat-error>First name is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Last Name</mat-label>
                <input matInput formControlName="lastName" placeholder="Doe">
                @if (empForm.controls.lastName.hasError('required')) {
                  <mat-error>Last name is required</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Email</mat-label>
                <input matInput type="email" formControlName="email" placeholder="john.doe@example.com">
                @if (empForm.controls.email.hasError('required')) {
                  <mat-error>Email is required</mat-error>
                }
                @if (empForm.controls.email.hasError('email')) {
                  <mat-error>Invalid email format</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Phone</mat-label>
                <input matInput type="tel" formControlName="phone" placeholder="+1234567890">
                @if (empForm.controls.phone.hasError('required')) {
                  <mat-error>Phone is required</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Department</mat-label>
                <mat-select formControlName="departmentId">
                  @for (dept of availableDepartments(); track dept.id) {
                    <mat-option [value]="dept.id">{{ dept.name }}</mat-option>
                  }
                </mat-select>
                @if (empForm.controls.departmentId.hasError('required')) {
                  <mat-error>Department is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Position</mat-label>
                <input matInput formControlName="position" placeholder="Software Engineer">
                @if (empForm.controls.position.hasError('required')) {
                  <mat-error>Position is required</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Hire Date</mat-label>
                <input matInput [matDatepicker]="picker" formControlName="hireDate">
                <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
                <mat-datepicker #picker></mat-datepicker>
                @if (empForm.controls.hireDate.hasError('required')) {
                  <mat-error>Hire date is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Salary</mat-label>
                <span matTextPrefix>$&nbsp;</span>
                <input matInput type="number" formControlName="salary" min="0">
                @if (empForm.controls.salary.hasError('required')) {
                  <mat-error>Salary is required</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/employees'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="empForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : (isEditMode() ? 'Update Employee' : 'Create Employee') }}
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

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-top: 16px;
    
      flex-wrap: wrap;
    }
  `]
})
export class EmployeeFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly employeesService = inject(EmployeesService);
  private readonly departmentsService = inject(DepartmentsService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);
  readonly employeeId = signal<number | null>(null);
  readonly availableDepartments = signal<Department[]>([]);

  readonly empForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    departmentId: [0 as number | null, Validators.required],
    position: ['', Validators.required],
    hireDate: [new Date(), Validators.required],
    salary: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    this.loadDepartments();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.employeeId.set(+id);
      this.loadEmployee(+id);
    }
  }
  
  private loadDepartments(): void {
    this.departmentsService.getDepartments({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.availableDepartments.set(res.items),
      error: () => this.notification.error('Failed to load departments')
    });
  }

  private loadEmployee(id: number): void {
    this.employeesService.getEmployee(id).subscribe({
      next: (emp) => {
        this.empForm.patchValue({
          firstName: emp.firstName,
          lastName: emp.lastName,
          email: emp.email,
          phone: emp.phone,
          departmentId: emp.departmentId,
          position: emp.position,
          hireDate: new Date(emp.hireDate),
          salary: emp.salary
        });
      },
      error: () => {
        this.notification.error('Failed to load employee details');
        this.router.navigate(['/admin/employees']);
      }
    });
  }

  onSubmit(): void {
    if (this.empForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.empForm.getRawValue();
    
    const requestData = {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      email: formValue.email,
      phone: formValue.phone,
      departmentId: formValue.departmentId as number,
      position: formValue.position,
      hireDate: new Date(formValue.hireDate).toISOString(),
      salary: formValue.salary
    };

    if (this.isEditMode() && this.employeeId()) {
      this.employeesService.updateEmployee(this.employeeId()!, { id: this.employeeId()!, ...requestData }).subscribe({
        next: () => {
          this.notification.success('Employee updated successfully');
          this.router.navigate(['/admin/employees']);
        },
        error: () => {
          this.notification.error('Failed to update employee');
          this.isSaving.set(false);
        }
      });
    } else {
      this.employeesService.createEmployee(requestData).subscribe({
        next: () => {
          this.notification.success('Employee created successfully');
          this.router.navigate(['/admin/employees']);
        },
        error: () => {
          this.notification.error('Failed to create employee');
          this.isSaving.set(false);
        }
      });
    }
  }
}
