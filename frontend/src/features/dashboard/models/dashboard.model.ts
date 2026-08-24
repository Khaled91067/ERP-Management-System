export interface DashboardMetrics {
  totalRevenue: number;
  totalOrders: number;
  totalProducts: number;
  totalCustomers: number;
}

export interface LowStockAlert {
  id: number;
  name: string;
  sku: string;
  stockQuantity: number;
  reorderLevel: number;
}

export interface RecentOrder {
  id: number;
  orderDate: string;
  customerName: string;
  status: string;
  totalAmount: number;
}

export interface DashboardData {
  metrics: DashboardMetrics;
  recentOrders: RecentOrder[];
  lowStockProducts: LowStockAlert[];
}
