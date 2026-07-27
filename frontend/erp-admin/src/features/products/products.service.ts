import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface Product {
  id: number;
  name: string;
  sku: string;
  categoryId: number;
  categoryName?: string;
  unitPrice: number;
  costPrice: number;
  stockQuantity: number;
  reorderLevel: number;
}

export interface AdjustStockDto {
  quantityChange: number;
}

export interface ProductPaginationParams extends PaginationParams {
  categoryId?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'products';

  getProducts(params?: ProductPaginationParams): Observable<PaginatedResult<Product>> {
    return this.apiService.getAll<Product>(this.endpoint, params as any);
  }

  getProduct(id: number): Observable<Product> {
    return this.apiService.getById<Product>(this.endpoint, id);
  }

  createProduct(data: Partial<Product>): Observable<number> {
    return this.apiService.create<Partial<Product>, number>(this.endpoint, data);
  }

  updateProduct(id: number, data: Partial<Product>): Observable<void> {
    return this.apiService.update<Partial<Product>, void>(this.endpoint, id, data);
  }

  deleteProduct(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }

  adjustStock(id: number, quantityChange: number): Observable<void> {
    return this.apiService.patch<AdjustStockDto, void>(this.endpoint, id, 'stock', { quantityChange });
  }
}
