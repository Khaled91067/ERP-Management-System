import { PaginationParams } from '@core/models/pagination.model';

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
}
