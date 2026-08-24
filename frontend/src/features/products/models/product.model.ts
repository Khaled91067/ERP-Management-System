import { PaginationParams } from '@core/models/pagination.model';

export interface Product {
  id: number;
  name: string;
  sku: string;
  description: string;
  unitPrice: number;
  stockQuantity: number;
  reorderLevel: number;
  categoryId: number;
  categoryName: string;
}

export interface AdjustStockDto {
  quantity: number;
  reason: string;
}

export interface ProductPaginationParams extends PaginationParams {
  categoryId?: number;
  lowStock?: boolean;
}
