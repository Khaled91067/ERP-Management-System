import { PaginationParams } from '@core/models/pagination.model';

export interface PurchaseOrderLine {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
}

export interface PurchaseOrder {
  id: number;
  supplierId: number;
  supplierName: string;
  orderDate: string;
  expectedDelivery: string;
  expectedDeliveryDate?: string;
  status: string;
  totalAmount: number;
  lines?: PurchaseOrderLine[];
}

export interface PurchaseOrderPaginationParams extends PaginationParams {
  supplierId?: number;
  status?: string;
}
