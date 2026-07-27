import { Routes } from '@angular/router';

export const PURCHASE_ORDERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./purchase-orders-list/purchase-orders-list.component').then(m => m.PurchaseOrdersListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./purchase-order-form/purchase-order-form.component').then(m => m.PurchaseOrderFormComponent)
  }
];
