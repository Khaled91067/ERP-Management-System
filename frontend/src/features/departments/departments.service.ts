import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface Department {
  id: number;
  name: string;
}

export interface DepartmentPaginationParams extends PaginationParams {
  search?: string;
}

@Injectable({
  providedIn: 'root'
})
export class DepartmentsService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'departments';

  getDepartments(params?: DepartmentPaginationParams): Observable<PaginatedResult<Department>> {
    return this.apiService.getAll<Department>(this.endpoint, params as any);
  }

  createDepartment(data: Partial<Department>): Observable<{ id: number }> {
    return this.apiService.create<Partial<Department>, { id: number }>(this.endpoint, data);
  }

  deleteDepartment(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
