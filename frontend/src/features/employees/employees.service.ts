import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  departmentId: number;
  departmentName: string;
  position: string;
  hireDate: string;
  salary: number;
}

export interface EmployeePaginationParams extends PaginationParams {
  departmentId?: number;
  searchTerm?: string;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeesService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'employees';

  getEmployees(params?: EmployeePaginationParams): Observable<PaginatedResult<Employee>> {
    return this.apiService.getAll<Employee>(this.endpoint, params as any);
  }

  getEmployee(id: number): Observable<Employee> {
    return this.apiService.getById<Employee>(this.endpoint, id);
  }

  createEmployee(data: Partial<Employee>): Observable<number> {
    return this.apiService.create<Partial<Employee>, number>(this.endpoint, data);
  }

  updateEmployee(id: number, data: Partial<Employee>): Observable<void> {
    return this.apiService.update<Partial<Employee>, void>(this.endpoint, id, data);
  }

  deleteEmployee(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
