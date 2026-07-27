import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog } from '@angular/material/dialog';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { CurrencyPipe } from '@angular/common';

import { ProductsService, Product } from '../products.service';
import { CategoriesService, Category } from '@features/categories/categories.service';
import { NotificationService } from '@core/services/notification.service';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-products-list',
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
    MatSelectModule,
    PageHeaderComponent,
    StatusBadgeComponent,
    CurrencyPipe
  ],
  template: `
    <div class="page-container">
      <app-page-header
        title="Products"
        [breadcrumbs]="[{ label: 'Inventory' }, { label: 'Products' }]"
        actionLabel="New Product"
        actionIcon="add"
        (action)="router.navigate(['/admin/products/new'])"
      />

      <div class="table-toolbar">
        <mat-form-field appearance="outline" class="search-field" subscriptSizing="dynamic">
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchControl" placeholder="Search products...">
        </mat-form-field>
        
        <mat-form-field appearance="outline" class="category-filter" subscriptSizing="dynamic">
          <mat-label>Category</mat-label>
          <mat-select [formControl]="categoryControl">
            <mat-option [value]="null">All Categories</mat-option>
            @for (category of categories(); track category.id) {
              <mat-option [value]="category.id">{{ category.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </div>

      <div class="table-container mat-elevation-z0">
        <table mat-table [dataSource]="products()" class="full-width">
          
          <ng-container matColumnDef="product">
            <th mat-header-cell *matHeaderCellDef>Product</th>
            <td mat-cell *matCellDef="let product">
              <div class="product-info">
                <span class="product-name">{{ product.name }}</span>
                <span class="product-sku">SKU: {{ product.sku }}</span>
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="category">
            <th mat-header-cell *matHeaderCellDef>Category</th>
            <td mat-cell *matCellDef="let product">{{ product.categoryName || 'N/A' }}</td>
          </ng-container>

          <ng-container matColumnDef="price">
            <th mat-header-cell *matHeaderCellDef class="text-right">Price</th>
            <td mat-cell *matCellDef="let product" class="text-right">{{ product.unitPrice | currency }}</td>
          </ng-container>

          <ng-container matColumnDef="stock">
            <th mat-header-cell *matHeaderCellDef>Stock</th>
            <td mat-cell *matCellDef="let product">
              <div class="stock-info">
                <span>{{ product.stockQuantity }}</span>
                @if (product.stockQuantity <= product.reorderLevel) {
                  <app-status-badge status="Low Stock" />
                }
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-column">Actions</th>
            <td mat-cell *matCellDef="let product" class="actions-column">
              <button mat-icon-button color="primary" [routerLink]="['/admin/products', product.id, 'edit']" matTooltip="Edit Product" aria-label="Edit product">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button color="warn" (click)="deleteProduct(product)" matTooltip="Delete Product" aria-label="Delete product">
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
                  <span class="empty-title">Loading products...</span>
                </div>
              } @else {
                <div class="table-empty-state">
                  <mat-icon>inventory_2</mat-icon>
                  <span class="empty-title">No products found</span>
                  <span class="empty-subtitle">Try adjusting your category filter or search term</span>
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
    }
    
    @media (max-width: 600px) {
      .table-toolbar {
        flex-direction: column;
      }
    }

    .search-field {
      flex: 1;
      max-width: 400px;
    }
    
    .category-filter {
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

    .product-info {
      display: flex;
      flex-direction: column;
      padding: 8px 0;
    }

    .product-name {
      font-weight: 500;
    }

    .product-sku {
      font-size: 0.75rem;
      color: var(--text-secondary);
    }
    
    .stock-info {
      display: flex;
      align-items: center;
      gap: 8px;
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
export class ProductsListComponent implements OnInit {
  private readonly productsService = inject(ProductsService);
  private readonly categoriesService = inject(CategoriesService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['product', 'category', 'price', 'stock', 'actions'];
  readonly products = signal<Product[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');
  readonly categoryControl = new FormControl<number | null>(null);

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadProducts();
      });
      
    this.categoryControl.valueChanges.subscribe(() => {
      this.pageIndex.set(0);
      this.loadProducts();
    });
  }
  
  loadCategories(): void {
    this.categoriesService.getCategories({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.categories.set(res.items)
    });
  }

  loadProducts(): void {
    this.isLoading.set(true);
    
    const categoryId = this.categoryControl.value;
    
    this.productsService.getProducts({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined,
      categoryId: categoryId !== null ? categoryId : undefined
    }).subscribe({
      next: (result) => {
        this.products.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load products');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadProducts();
  }

  deleteProduct(product: Product): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Product',
        message: 'Are you sure you want to delete this product?',
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.productsService.deleteProduct(product.id).subscribe({
          next: () => {
            this.notification.success('Product deleted successfully');
            this.loadProducts();
          },
          error: () => this.notification.error('Failed to delete product')
        });
      }
    });
  }
}
