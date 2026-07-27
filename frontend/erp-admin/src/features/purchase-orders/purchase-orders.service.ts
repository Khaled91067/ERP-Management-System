import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface PurchaseOrder {
  id: number;
  supplierId: number;
  orderDate: string;
  expectedDelivery: string;
  status: string;
  totalAmount: number;
}

export interface PurchaseOrderLineDto {
  productId: number;
  quantity: number;
  unitCost: number;
}

export interface PurchaseOrderPaginationParams extends PaginationParams {
  supplierId?: number;
  status?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrdersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'purchase-orders';

  getPurchaseOrders(params?: PurchaseOrderPaginationParams): Observable<PaginatedResult<PurchaseOrder>> {
    return this.apiService.getAll<PurchaseOrder>(this.endpoint, params as any);
  }

  getPurchaseOrder(id: number): Observable<PurchaseOrder> {
    return this.apiService.getById<PurchaseOrder>(this.endpoint, id);
  }

  createPurchaseOrder(data: Partial<PurchaseOrder>): Observable<number> {
    return this.apiService.create<Partial<PurchaseOrder>, number>(this.endpoint, data);
  }

  updateStatus(id: number, status: string): Observable<void> {
    return this.apiService.patch<any, void>(this.endpoint, id, 'status', { status });
  }
}
