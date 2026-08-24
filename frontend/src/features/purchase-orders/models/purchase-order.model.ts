import { PaginationParams } from '@core/models/pagination.model';

export interface PurchaseOrder {
  id: number;
  supplierId: number;
  supplierName: string;
  orderDate: string;
  expectedDeliveryDate: string;
  status: string;
  totalAmount: number;
}

export interface PurchaseOrderPaginationParams extends PaginationParams {
  supplierId?: number;
  status?: string;
}
