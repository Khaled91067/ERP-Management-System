import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult, PaginationParams } from '@core/models/pagination.model';

export interface UserDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  isActive: boolean;
  role: string;
}

export interface ChangeRoleDto {
  roleId: number;
}

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'admin/users';

  getUsers(params?: PaginationParams): Observable<PaginatedResult<UserDto>> {
    return this.apiService.getAll<UserDto>(this.endpoint, params as any);
  }

  getUser(id: number): Observable<UserDto> {
    return this.apiService.getById<UserDto>(this.endpoint, id);
  }

  createUser(data: any): Observable<number> {
    return this.apiService.create<any, number>(this.endpoint, data);
  }

  updateUser(id: number, data: any): Observable<void> {
    return this.apiService.update<any, void>(this.endpoint, id, data);
  }

  changeRole(id: number, roleId: number): Observable<void> {
    return this.apiService.patch<ChangeRoleDto, void>(this.endpoint, id, 'role', { roleId });
  }

  deleteUser(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
