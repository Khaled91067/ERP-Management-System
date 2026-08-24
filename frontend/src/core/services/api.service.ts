import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { APP_CONFIG } from '@core/config/app-config.token';
import { PaginatedResult } from '@core/models/api-response.model';
import { PaginationParams } from '@core/models/pagination.model';

/**
 * Generic base service for CRUD operations against the ERP REST API.
 * Feature services should inject this and delegate HTTP calls to it,
 * or extend it if they need resource-specific methods.
 */
@Injectable({ providedIn: 'root' })
export class ApiService {
  protected readonly http = inject(HttpClient);
  private readonly config = inject(APP_CONFIG);

  protected url(path: string): string {
    return `${this.config.apiUrl}/${path}`;
  }

  /** GET /resource — paginated list with optional search/filter params */
  getAll<T>(
    resource: string,
    params?: PaginationParams | Record<string, unknown>
  ): Observable<PaginatedResult<T>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          httpParams = httpParams.set(key, String(value));
        }
      });
    }
    return this.http.get<any>(this.url(resource), {
      params: httpParams,
    }).pipe(
      map(res => {
        if (res && typeof res === 'object') {
          return {
            ...res,
            total: res.total ?? res.totalCount ?? (Array.isArray(res.items) ? res.items.length : 0),
            items: res.items ?? []
          };
        }
        return res;
      })
    );
  }

  /** GET /resource/:id */
  getById<T>(resource: string, id: string | number): Observable<T> {
    return this.http.get<T>(`${this.url(resource)}/${id}`);
  }

  /** POST /resource */
  create<TBody, TResponse = TBody>(
    resource: string,
    body: TBody
  ): Observable<TResponse> {
    return this.http.post<TResponse>(this.url(resource), body);
  }

  /** PUT /resource/:id */
  update<TBody, TResponse = TBody>(
    resource: string,
    id: string | number,
    body: TBody
  ): Observable<TResponse> {
    return this.http.put<TResponse>(`${this.url(resource)}/${id}`, body);
  }

  /** PATCH /resource/:id/action */
  patch<TBody, TResponse = TBody>(
    resource: string,
    id: string | number,
    action: string,
    body?: TBody
  ): Observable<TResponse> {
    return this.http.patch<TResponse>(
      `${this.url(resource)}/${id}/${action}`,
      body ?? {}
    );
  }

  /** DELETE /resource/:id */
  delete<TResponse = void>(
    resource: string,
    id: string | number
  ): Observable<TResponse> {
    return this.http.delete<TResponse>(`${this.url(resource)}/${id}`);
  }

  /** GET /resource/... (for downloading blobs) */
  downloadFile(resource: string, path: string): Observable<Blob> {
    return this.http.get(`${this.url(resource)}/${path}`, {
      responseType: 'blob'
    });
  }
}
