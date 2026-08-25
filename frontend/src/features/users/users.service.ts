import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';
import { PaginatedResult } from '@core/models/api-response.model';
import { PaginationParams } from '@core/models/pagination.model';
import { ChangeRoleDto, UserDto } from './models/user-management.model';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'admin/users';

  getUsers(params?: PaginationParams): Observable<PaginatedResult<UserDto>> {
    return this.apiService.getAll<UserDto>(this.endpoint, params);
  }

  getUser(id: number): Observable<UserDto> {
    return this.apiService.getById<UserDto>(this.endpoint, id);
  }

  createUser(data: any): Observable<{ id: number }> {
    return this.apiService.create<any, { id: number }>(this.endpoint, data);
  }

  updateUser(id: number, data: Partial<UserDto>): Observable<void> {
    return this.apiService.update<Partial<UserDto>, void>(this.endpoint, id, data);
  }

  changeRole(id: number, roleId: number): Observable<void> {
    return this.apiService.patch<ChangeRoleDto, void>(this.endpoint, id, 'role', { roleId });
  }

  deleteUser(id: number): Observable<void> {
    return this.apiService.delete<void>(this.endpoint, id);
  }
}
