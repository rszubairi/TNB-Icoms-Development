import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  private unwrap<T>(response$: Observable<ApiResponse<T>>): Observable<T> {
    return response$.pipe(
      map((response) => {
        if (!response.success) {
          throw new Error(response.error ?? 'Request failed');
        }
        return response.data;
      })
    );
  }

  get<T>(path: string, params?: Record<string, string | number | boolean | undefined>): Observable<T> {
    const httpParams: Record<string, string> = {};
    if (params) {
      for (const key of Object.keys(params)) {
        const value = params[key];
        if (value !== undefined && value !== null) {
          httpParams[key] = String(value);
        }
      }
    }
    return this.unwrap(this.http.get<ApiResponse<T>>(`${this.baseUrl}${path}`, { params: httpParams }));
  }

  post<T>(path: string, body: unknown): Observable<T> {
    return this.unwrap(this.http.post<ApiResponse<T>>(`${this.baseUrl}${path}`, body));
  }

  put<T>(path: string, body: unknown): Observable<T> {
    return this.unwrap(this.http.put<ApiResponse<T>>(`${this.baseUrl}${path}`, body));
  }

  delete<T>(path: string): Observable<T> {
    return this.unwrap(this.http.delete<ApiResponse<T>>(`${this.baseUrl}${path}`));
  }
}
