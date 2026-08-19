import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateDropdownValueRequest,
  DropdownCategoriesResponse,
  DropdownValue,
  UpdateDropdownValueRequest
} from '../models/dropdown-value.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class DropdownValueService {
  private api = inject(ApiService);

  listByCategory(category: string): Observable<DropdownValue[]> {
    return this.api.get<DropdownValue[]>('/dropdown-values', { category });
  }

  listCategories(): Observable<DropdownCategoriesResponse> {
    return this.api.get<DropdownCategoriesResponse>('/dropdown-values/categories');
  }

  listForAdmin(category: string): Observable<DropdownValue[]> {
    return this.api.get<DropdownValue[]>('/dropdown-values/admin', { category });
  }

  create(request: CreateDropdownValueRequest): Observable<DropdownValue> {
    return this.api.post<DropdownValue>('/dropdown-values', request);
  }

  update(dropdownValueId: number, request: UpdateDropdownValueRequest): Observable<DropdownValue> {
    return this.api.put<DropdownValue>(`/dropdown-values/${dropdownValueId}`, request);
  }

  reorder(dropdownValueId: number, direction: 'up' | 'down'): Observable<unknown> {
    return this.api.post(`/dropdown-values/${dropdownValueId}/reorder`, { direction });
  }

  deactivate(dropdownValueId: number): Observable<unknown> {
    return this.api.delete(`/dropdown-values/${dropdownValueId}`);
  }
}
