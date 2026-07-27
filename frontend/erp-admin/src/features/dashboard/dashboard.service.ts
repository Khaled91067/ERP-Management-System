import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable, forkJoin, map } from 'rxjs';

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

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiService = inject(ApiService);

  getDashboardData(): Observable<DashboardData> {
    const orders$ = this.apiService.getAll<any>('orders', { page: 1, pageSize: 100 });
    const products$ = this.apiService.getAll<any>('products', { page: 1, pageSize: 100 });
    const customers$ = this.apiService.getAll<any>('customers', { page: 1, pageSize: 1 });

    return forkJoin([orders$, products$, customers$]).pipe(
      map(([orders, products, customers]) => {
        const orderItems = orders?.items || [];
        const productItems = products?.items || [];

        // Calculate metrics
        const totalRevenue = orderItems.reduce((sum: number, order: any) => sum + (order.totalAmount || 0), 0);
        
        // Find low stock products
        const lowStockProducts = productItems
          .filter((p: any) => (p.stockQuantity ?? 0) <= (p.reorderLevel ?? 0))
          .slice(0, 5)
          .map((p: any) => ({
            id: p.id,
            name: p.name || 'Unnamed Product',
            sku: p.sku || 'N/A',
            stockQuantity: p.stockQuantity ?? 0,
            reorderLevel: p.reorderLevel ?? 0
          }));

        // Get recent orders
        const recentOrders = [...orderItems]
          .sort((a: any, b: any) => new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime())
          .slice(0, 5)
          .map((o: any) => ({
            id: o.id,
            orderDate: o.orderDate,
            customerName: o.customerName || `Customer #${o.customerId}`,
            status: o.status || 'PENDING',
            totalAmount: o.totalAmount || 0
          }));

        return {
          metrics: {
            totalRevenue,
            totalOrders: orders?.total ?? orderItems.length,
            totalProducts: products?.total ?? productItems.length,
            totalCustomers: customers?.total ?? 0
          },
          recentOrders,
          lowStockProducts
        };
      })
    );
  }
}
