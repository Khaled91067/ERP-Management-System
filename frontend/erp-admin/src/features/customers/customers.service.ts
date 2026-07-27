import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface Customer {
  id: number;
  name: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  country: string;
  taxId: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'customers';

  getCustomers(params?: PaginationParams): Observable<PaginatedResult<Customer>> {
    return this.apiService.getAll<Customer>(this.endpoint, params as any);
  }

  getCustomer(id: number): Observable<Customer> {
    return this.apiService.getById<Customer>(this.endpoint, id);
  }

  createCustomer(data: Partial<Customer>): Observable<number> {
    return this.apiService.create<Partial<Customer>, number>(this.endpoint, data);
  }

  updateCustomer(id: number, data: Partial<Customer>): Observable<void> {
    return this.apiService.update<Partial<Customer>, void>(this.endpoint, id, data);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
