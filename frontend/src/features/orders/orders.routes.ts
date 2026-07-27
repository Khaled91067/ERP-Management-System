import { Routes } from '@angular/router';

export const ORDERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./orders-list/orders-list.component').then(m => m.OrdersListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./order-form/order-form.component').then(m => m.OrderFormComponent)
  }
];
