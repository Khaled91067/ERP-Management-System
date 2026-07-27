import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface OrderLine {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  discountPercentage: number;
  totalPrice: number;
}

export interface Order {
  id: number;
  customerId: number;
  customerName: string;
  orderDate: string;
  status: string;
  paymentMethod: string;
  shippingAddress: string;
  totalAmount: number;
  lines: OrderLine[];
}

export interface OrderPaginationParams extends PaginationParams {
  customerId?: number;
  status?: string;
  searchTerm?: string;
}

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'orders';

  getOrders(params?: OrderPaginationParams): Observable<PaginatedResult<Order>> {
    return this.apiService.getAll<Order>(this.endpoint, params as any);
  }

  getOrder(id: number): Observable<Order> {
    return this.apiService.getById<Order>(this.endpoint, id);
  }

  createOrder(data: Partial<Order>): Observable<number> {
    return this.apiService.create<Partial<Order>, number>(this.endpoint, data);
  }

  updateStatus(id: number, status: string): Observable<void> {
    return this.apiService.update<any, void>(this.endpoint, `${id}/status`, { orderId: id, status });
  }

  deleteOrder(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
