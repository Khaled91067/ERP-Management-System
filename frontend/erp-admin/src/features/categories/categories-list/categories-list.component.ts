import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialog } from '@angular/material/dialog';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { CategoriesService, Category } from '../categories.service';
import { NotificationService } from '@core/services/notification.service';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-categories-list',
  standalone: true,
  imports: [
    RouterLink,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Categories"
        [breadcrumbs]="[{ label: 'Inventory' }, { label: 'Categories' }]"
        actionLabel="New Category"
        actionIcon="add"
        (action)="router.navigate(['/admin/categories/new'])"
      />

      <div class="table-toolbar">
        <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchControl" placeholder="Search categories...">
        </mat-form-field>
      </div>

      <div class="table-container mat-elevation-z0">
        <table mat-table [dataSource]="categories()" class="full-width">
          
          <ng-container matColumnDef="id">
            <th mat-header-cell *matHeaderCellDef>ID</th>
            <td mat-cell *matCellDef="let category">{{ category.id }}</td>
          </ng-container>

          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef>Category Name</th>
            <td mat-cell *matCellDef="let category">{{ category.name }}</td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-column">Actions</th>
            <td mat-cell *matCellDef="let category" class="actions-column">
              <button mat-icon-button color="primary" [routerLink]="['/admin/categories', category.id, 'edit']" matTooltip="Edit">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button color="warn" (click)="deleteCategory(category)" matTooltip="Delete">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
          
          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell empty-cell" [attr.colspan]="columns.length">
              @if (isLoading()) {
                Loading categories...
              } @else {
                No categories found
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
      margin-bottom: 16px;
    }

    .search-field {
      width: 100%;
      max-width: 400px;
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

    .actions-column {
      width: 120px;
      text-align: right;
    }

    .empty-cell {
      text-align: center;
      padding: 48px;
      color: var(--text-secondary);
    }
  `]
})
export class CategoriesListComponent implements OnInit {
  private readonly categoriesService = inject(CategoriesService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['id', 'name', 'actions'];
  readonly categories = signal<Category[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');

  ngOnInit(): void {
    this.loadCategories();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadCategories();
      });
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.categoriesService.getCategories({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined
    }).subscribe({
      next: (result) => {
        this.categories.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load categories');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadCategories();
  }

  deleteCategory(category: Category): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Category',
        message: 'Are you sure you want to delete this category? Products in this category may be affected.',
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.categoriesService.deleteCategory(category.id).subscribe({
          next: () => {
            this.notification.success('Category deleted successfully');
            this.loadCategories();
          },
          error: () => this.notification.error('Failed to delete category')
        });
      }
    });
  }
}
