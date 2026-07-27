import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface InvoiceLine {
  id: number;
  description: string;
  quantity: number;
  unitPrice: number;
  taxRate: number;
  totalPrice: number;
}

export interface Invoice {
  id: number;
  orderId?: number;
  customerId: number;
  customerName: string;
  invoiceDate: string;
  dueDate: string;
  status: string;
  totalAmount: number;
  paidAt?: string;
  lines: InvoiceLine[];
}

export interface GenerateInvoiceDto {
  orderId: number;
  dueDate: string;
}

export interface InvoicePaginationParams extends PaginationParams {
  customerId?: number;
  status?: string;
  search?: string;
}

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'invoices';

  getInvoices(params?: InvoicePaginationParams): Observable<PaginatedResult<Invoice>> {
    return this.apiService.getAll<Invoice>(this.endpoint, params as any);
  }

  getInvoice(id: number): Observable<Invoice> {
    return this.apiService.getById<Invoice>(this.endpoint, id);
  }

  createInvoice(data: Partial<Invoice>): Observable<number> {
    return this.apiService.create<Partial<Invoice>, number>(this.endpoint, data);
  }

  generateFromOrder(data: GenerateInvoiceDto): Observable<number> {
    return this.apiService.create<GenerateInvoiceDto, number>(`${this.endpoint}/generate`, data);
  }

  payInvoice(id: number): Observable<void> {
    return this.apiService.update<{ invoiceId: number }, void>(this.endpoint, `${id}/pay`, { invoiceId: id });
  }

  deleteInvoice(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
