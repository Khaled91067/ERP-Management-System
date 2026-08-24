import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult } from '@core/models/api-response.model';
import { PaginationParams } from '@core/models/pagination.model';
import { Category } from './models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'categories';

  getCategories(params?: PaginationParams): Observable<PaginatedResult<Category>> {
    return this.apiService.getAll<Category>(this.endpoint, params);
  }

  getCategory(id: number): Observable<Category> {
    return this.apiService.getById<Category>(this.endpoint, id);
  }

  createCategory(data: Partial<Category>): Observable<number> {
    return this.apiService.create<Partial<Category>, number>(this.endpoint, data);
  }

  updateCategory(id: number, data: Partial<Category>): Observable<void> {
    return this.apiService.update<Partial<Category>, void>(this.endpoint, id, data);
  }

  deleteCategory(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
