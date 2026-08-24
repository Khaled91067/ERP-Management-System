import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult } from '@core/models/api-response.model';
import { AdjustStockDto, Product, ProductPaginationParams } from './models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'products';

  getProducts(params?: ProductPaginationParams): Observable<PaginatedResult<Product>> {
    return this.apiService.getAll<Product>(this.endpoint, params);
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

  adjustStock(id: number, data: AdjustStockDto): Observable<void> {
    const quantityChange = data.quantity ?? (data as any).quantityChange ?? 0;
    return this.apiService.patch<{ quantityChange: number }, void>(this.endpoint, id, 'stock', { quantityChange });
  }
}
