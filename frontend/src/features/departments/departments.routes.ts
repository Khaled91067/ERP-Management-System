import { Routes } from '@angular/router';

export const DEPARTMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./departments-list/departments-list.component').then(m => m.DepartmentsListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./department-form/department-form.component').then(m => m.DepartmentFormComponent)
  }
];
