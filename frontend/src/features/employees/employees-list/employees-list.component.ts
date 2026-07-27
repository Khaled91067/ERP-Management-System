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

import { EmployeesService, Employee } from '../employees.service';
import { DepartmentsService, Department } from '@features/departments/departments.service';
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
  template: `
    <div class="page-container">
      <app-page-header
        title="Employees"
        [breadcrumbs]="[{ label: 'HR' }, { label: 'Employees' }]"
        actionLabel="New Employee"
        actionIcon="person_add"
        (action)="router.navigate(['/admin/employees/new'])"
      />

      <div class="table-toolbar">
        <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchControl" placeholder="Search employees...">
        </mat-form-field>
        
        <mat-form-field appearance="outline" class="filter-field" subscriptSizing="dynamic">
          <mat-label>Department</mat-label>
          <mat-select [formControl]="departmentControl">
            <mat-option [value]="null">All Departments</mat-option>
            @for (dept of departments(); track dept.id) {
              <mat-option [value]="dept.id">{{ dept.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </div>

      <div class="table-container mat-elevation-z0">
        <table mat-table [dataSource]="employees()" class="full-width">
          
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef>Name</th>
            <td mat-cell *matCellDef="let emp">
              <div class="emp-name">{{ emp.firstName }} {{ emp.lastName }}</div>
              <div class="emp-email">{{ emp.email }}</div>
            </td>
          </ng-container>

          <ng-container matColumnDef="role">
            <th mat-header-cell *matHeaderCellDef>Role</th>
            <td mat-cell *matCellDef="let emp">
              <div class="emp-position">{{ emp.position }}</div>
              <div class="emp-dept">{{ emp.departmentName }}</div>
            </td>
          </ng-container>

          <ng-container matColumnDef="hireDate">
            <th mat-header-cell *matHeaderCellDef>Hire Date</th>
            <td mat-cell *matCellDef="let emp">{{ emp.hireDate | date:'mediumDate' }}</td>
          </ng-container>

          <ng-container matColumnDef="salary">
            <th mat-header-cell *matHeaderCellDef class="text-right">Salary</th>
            <td mat-cell *matCellDef="let emp" class="text-right">{{ emp.salary | currency }}</td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-column">Actions</th>
            <td mat-cell *matCellDef="let emp" class="actions-column">
              <a mat-icon-button color="primary" [routerLink]="['/admin/employees', emp.id, 'edit']" matTooltip="Edit Employee" aria-label="Edit employee">
                <mat-icon>edit</mat-icon>
              </a>
              <button mat-icon-button color="warn" (click)="deleteEmployee(emp)" matTooltip="Delete Employee" aria-label="Delete employee">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
          
          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell empty-cell" [attr.colspan]="columns.length">
              @if (isLoading()) {
                <div class="table-empty-state">
                  <mat-icon>sync</mat-icon>
                  <span class="empty-title">Loading employees...</span>
                </div>
              } @else {
                <div class="table-empty-state">
                  <mat-icon>badge</mat-icon>
                  <span class="empty-title">No employees found</span>
                  <span class="empty-subtitle">Try adjusting your filters or search</span>
                </div>
              }
            </td>
          </tr>
        </table>

        <mat-paginator
          [length]="totalItems()"
          [pageSize]="pageSize()"
          [pageIndex]="pageIndex()"
          [pageSizeOptions]="[10, 20, 50]"
          (page)="onPageChange($event)"
          showFirstLastButtons>
        </mat-paginator>
      </div>
    </div>
  `,
  styles: [`
    .table-toolbar {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
      flex-wrap: wrap;
    }
    
    @media (max-width: 768px) {
      .table-toolbar {
        flex-direction: column;
      }
    }

    .search-field {
      flex: 1;
      max-width: 400px;
    }
    
    .filter-field {
      width: 200px;
    }

    .table-container {
      background-color: var(--surface-card);
      border-radius: 12px;
      overflow-x: auto;
      border: 1px solid var(--border-color);
    }

    .full-width {
      width: 100%;
    }

    .emp-name {
      font-weight: 500;
    }

    .emp-email, .emp-dept {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }
    
    .emp-position {
      font-weight: 500;
    }

    .actions-column {
      width: 120px;
      text-align: right;
    }

    .empty-cell {
      text-align: center;
      padding: 24px;
    }
  `]
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
