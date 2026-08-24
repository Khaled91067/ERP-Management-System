import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult } from '@core/models/api-response.model';
import { PurchaseOrder, PurchaseOrderPaginationParams } from './models/purchase-order.model';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrdersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'purchase-orders';

  getPurchaseOrders(params?: PurchaseOrderPaginationParams): Observable<PaginatedResult<PurchaseOrder>> {
    return this.apiService.getAll<PurchaseOrder>(this.endpoint, params);
  }

  getPurchaseOrder(id: number): Observable<PurchaseOrder> {
    return this.apiService.getById<PurchaseOrder>(this.endpoint, id);
  }

  createPurchaseOrder(data: Partial<PurchaseOrder>): Observable<number> {
    return this.apiService.create<Partial<PurchaseOrder>, number>(this.endpoint, data);
  }

  receiveOrder(id: number): Observable<void> {
    return this.apiService.patch<{ status: string }, void>(this.endpoint, id, 'status', { status: 'Received' });
  }

  cancelOrder(id: number): Observable<void> {
    return this.apiService.patch<{ status: string }, void>(this.endpoint, id, 'status', { status: 'Cancelled' });
  }

  deletePurchaseOrder(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
