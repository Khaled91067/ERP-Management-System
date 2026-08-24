import { PaginationParams } from '@core/models/pagination.model';

export interface OrderLine {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Order {
  id: number;
  customerId: number;
  customerName: string;
  orderDate: string;
  status: string;
  totalAmount: number;
  lines: OrderLine[];
}

export interface OrderPaginationParams extends PaginationParams {
  customerId?: number;
  status?: string;
}
