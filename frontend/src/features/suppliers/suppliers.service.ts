import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface Supplier {
  id: number;
  companyName: string;
  contactName: string;
  email: string;
  phone: string;
  paymentTerms: string;
}

@Injectable({
  providedIn: 'root'
})
export class SuppliersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'suppliers';

  getSuppliers(params?: PaginationParams): Observable<PaginatedResult<Supplier>> {
    return this.apiService.getAll<Supplier>(this.endpoint, params as any);
  }

  getSupplier(id: number): Observable<Supplier> {
    return this.apiService.getById<Supplier>(this.endpoint, id);
  }

  createSupplier(data: Partial<Supplier>): Observable<number> {
    return this.apiService.create<Partial<Supplier>, number>(this.endpoint, data);
  }

  updateSupplier(id: number, data: Partial<Supplier>): Observable<void> {
    return this.apiService.update<Partial<Supplier>, void>(this.endpoint, id, data);
  }

  deleteSupplier(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
