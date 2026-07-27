import { Routes } from '@angular/router';
import { authGuard } from '@core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'admin',
    pathMatch: 'full'
  },
  {
    path: 'admin',
    loadComponent: () => import('@layout/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      // Overview
      {
        path: 'dashboard',
        loadComponent: () => import('@features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      // Sales
      {
        path: 'orders',
        loadChildren: () => import('@features/orders/orders.routes').then(m => m.ORDERS_ROUTES)
      },
      {
        path: 'customers',
        loadChildren: () => import('@features/customers/customers.routes').then(m => m.CUSTOMERS_ROUTES)
      },
      // Inventory
      {
        path: 'products',
        loadChildren: () => import('@features/products/products.routes').then(m => m.PRODUCTS_ROUTES)
      },
      {
        path: 'categories',
        loadChildren: () => import('@features/categories/categories.routes').then(m => m.CATEGORIES_ROUTES)
      },
      // Purchasing
      {
        path: 'suppliers',
        loadChildren: () => import('@features/suppliers/suppliers.routes').then(m => m.SUPPLIERS_ROUTES)
      },
      {
        path: 'purchase-orders',
        loadChildren: () => import('@features/purchase-orders/purchase-orders.routes').then(m => m.PURCHASE_ORDERS_ROUTES)
      },
      // Finance
      {
        path: 'invoices',
        loadChildren: () => import('@features/invoices/invoices.routes').then(m => m.INVOICES_ROUTES)
      },
      // HR
      {
        path: 'employees',
        loadChildren: () => import('@features/employees/employees.routes').then(m => m.EMPLOYEES_ROUTES)
      },
      {
        path: 'departments',
        loadChildren: () => import('@features/departments/departments.routes').then(m => m.DEPARTMENTS_ROUTES)
      },
      // Settings
      {
        path: 'users',
        loadChildren: () => import('@features/users/users.routes').then(m => m.USERS_ROUTES)
      },
      {
        path: 'roles',
        loadChildren: () => import('@features/roles/roles.routes').then(m => m.ROLES_ROUTES)
      }
    ]
  },
  {
    path: 'auth',
    loadComponent: () => import('@layout/auth-layout/auth-layout.component').then(m => m.AuthLayoutComponent),
    children: [
      {
        path: '',
        loadChildren: () => import('@features/auth/auth.routes').then(m => m.AUTH_ROUTES)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'admin'
  }
];
