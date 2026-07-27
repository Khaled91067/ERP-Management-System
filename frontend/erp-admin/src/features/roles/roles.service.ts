import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface Role {
  id: number;
  name: string;
  normalizedName: string;
  permissions: string;
}

@Injectable({
  providedIn: 'root'
})
export class RolesService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'admin/roles';

  getRoles(params?: PaginationParams): Observable<PaginatedResult<Role>> {
    return this.apiService.getAll<Role>(this.endpoint, params as any);
  }

  getRole(id: number): Observable<Role> {
    return this.apiService.getById<Role>(this.endpoint, id);
  }

  createRole(data: Partial<Role>): Observable<number> {
    return this.apiService.create<Partial<Role>, number>(this.endpoint, data);
  }

  updateRole(id: number, data: Partial<Role>): Observable<void> {
    return this.apiService.update<Partial<Role>, void>(this.endpoint, id, data);
  }

  deleteRole(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
