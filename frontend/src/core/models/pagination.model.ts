import { PaginatedResult } from './api-response.model';

export type { PaginatedResult };

export interface PaginationParams {
  page: number;
  pageSize: number;
  search?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export type { ApiResponse } from './api-response.model';
