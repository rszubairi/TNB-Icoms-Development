import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DropdownValue } from '../models/dropdown-value.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class DropdownValueService {
  private api = inject(ApiService);

  listByCategory(category: string): Observable<DropdownValue[]> {
    return this.api.get<DropdownValue[]>('/dropdown-values', { category });
  }
}
