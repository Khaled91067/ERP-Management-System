import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult } from '@core/models/api-response.model';
import { Order, OrderPaginationParams } from './models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'orders';

  getOrders(params?: OrderPaginationParams): Observable<PaginatedResult<Order>> {
    return this.apiService.getAll<Order>(this.endpoint, params);
  }

  getOrder(id: number): Observable<Order> {
    return this.apiService.getById<Order>(this.endpoint, id);
  }

  createOrder(data: Partial<Order>): Observable<number> {
    return this.apiService.create<Partial<Order>, number>(this.endpoint, data);
  }

  updateOrderStatus(id: number, status: string): Observable<void> {
    return this.apiService.update<{ orderId: number; status: string }, void>(this.endpoint, `${id}/status`, { orderId: id, status });
  }

  deleteOrder(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
