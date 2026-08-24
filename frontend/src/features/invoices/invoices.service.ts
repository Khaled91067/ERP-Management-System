import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult } from '@core/models/api-response.model';
import { GenerateInvoiceDto, Invoice, InvoicePaginationParams } from './models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'invoices';

  getInvoices(params?: InvoicePaginationParams): Observable<PaginatedResult<Invoice>> {
    return this.apiService.getAll<Invoice>(this.endpoint, params);
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

  downloadInvoicePdf(id: number): Observable<Blob> {
    return this.apiService.downloadFile(this.endpoint, `${id}/pdf`);
  }
}
