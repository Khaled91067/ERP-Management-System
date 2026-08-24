import { PaginationParams } from '@core/models/pagination.model';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  departmentId: number;
  departmentName: string;
  position: string;
  hireDate: string;
  salary: number;
}

export interface EmployeePaginationParams extends PaginationParams {
  departmentId?: number;
  searchTerm?: string;
}
