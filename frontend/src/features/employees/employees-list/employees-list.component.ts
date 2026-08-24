import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { CurrencyPipe, DatePipe } from '@angular/common';

import { EmployeesService } from '../employees.service';
import { Employee } from '../models/employee.model';
import { DepartmentsService } from '@features/departments/departments.service';
import { Department } from '@features/departments/models/department.model';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-employees-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDialogModule,
    MatTooltipModule,
    PageHeaderComponent,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './employees-list.component.html',
  styleUrl: './employees-list.component.scss'
})
export class EmployeesListComponent implements OnInit {
  private readonly employeesService = inject(EmployeesService);
  private readonly departmentsService = inject(DepartmentsService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['name', 'role', 'hireDate', 'salary', 'actions'];
  readonly employees = signal<Employee[]>([]);
  readonly departments = signal<Department[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');
  readonly departmentControl = new FormControl<number | null>(null);

  ngOnInit(): void {
    this.loadDepartments();
    this.loadEmployees();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadEmployees();
      });
      
    this.departmentControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadEmployees();
    });
  }
  
  loadDepartments(): void {
    this.departmentsService.getDepartments({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.departments.set(res.items)
    });
  }

  loadEmployees(): void {
    this.isLoading.set(true);
    
    const departmentId = this.departmentControl.value;
    
    this.employeesService.getEmployees({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      searchTerm: this.searchControl.value || undefined,
      departmentId: departmentId !== null ? departmentId : undefined
    }).subscribe({
      next: (result) => {
        this.employees.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load employees');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadEmployees();
  }

  deleteEmployee(emp: Employee): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Employee',
        message: `Are you sure you want to delete ${emp.firstName} ${emp.lastName}?`,
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.employeesService.deleteEmployee(emp.id).subscribe({
          next: () => {
            this.notification.success('Employee deleted successfully');
            this.loadEmployees();
          },
          error: () => this.notification.error('Failed to delete employee')
        });
      }
    });
  }
}
