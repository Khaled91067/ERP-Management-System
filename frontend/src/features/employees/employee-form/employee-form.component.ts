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
import { DepartmentsService } from '@features/departments/departments.service';
import { Department } from '@features/departments/models/department.model';
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { parseDateSafe, toDateOnlyIsoString } from '@shared/pipes/localized-formatting.pipes';

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
    PageHeaderComponent,
    TranslatePipe
  ],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.scss'
})
export class EmployeeFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly employeesService = inject(EmployeesService);
  private readonly departmentsService = inject(DepartmentsService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  private readonly i18n = inject(TranslationService);
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
    departmentId: [null as number | null, Validators.required],
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
      next: (res) => this.availableDepartments.set(res.items)
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
          hireDate: parseDateSafe(emp.hireDate) ?? new Date(emp.hireDate),
          salary: emp.salary
        });
      },
      error: () => {
        this.router.navigate(['/admin/employees']);
      }
    });
  }

  onSubmit(): void {
    this.formValidation.prepareFormForSubmission(this.empForm);
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
      hireDate: toDateOnlyIsoString(formValue.hireDate),
      salary: formValue.salary
    };

    if (this.isEditMode() && this.employeeId()) {
      this.employeesService.updateEmployee(this.employeeId()!, { id: this.employeeId()!, ...requestData }).subscribe({
        next: () => {
          this.notification.success(this.i18n.translate('employees.updatedSuccess'));
          this.router.navigate(['/admin/employees']);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      this.employeesService.createEmployee(requestData).subscribe({
        next: () => {
          this.notification.success(this.i18n.translate('employees.createdSuccess'));
          this.router.navigate(['/admin/employees']);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }
}
