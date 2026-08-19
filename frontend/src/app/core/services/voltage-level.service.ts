import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateVoltageLevelRequest, UpdateVoltageLevelRequest, VoltageLevel } from '../models/voltage-level.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class VoltageLevelService {
  private api = inject(ApiService);

  list(): Observable<VoltageLevel[]> {
    return this.api.get<VoltageLevel[]>('/voltage-levels');
  }

  create(request: CreateVoltageLevelRequest): Observable<VoltageLevel> {
    return this.api.post<VoltageLevel>('/voltage-levels', request);
  }

  update(voltageLevelId: number, request: UpdateVoltageLevelRequest): Observable<VoltageLevel> {
    return this.api.put<VoltageLevel>(`/voltage-levels/${voltageLevelId}`, request);
  }

  deactivate(voltageLevelId: number): Observable<unknown> {
    return this.api.delete(`/voltage-levels/${voltageLevelId}`);
  }
}
