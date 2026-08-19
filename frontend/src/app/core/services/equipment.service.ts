import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateEquipmentRequest, Equipment, UpdateEquipmentRequest } from '../models/equipment.model';
import { ApiService } from './api.service';

export interface EquipmentListFilter {
  zoneId?: number;
  stationId?: number;
  voltageLevelId?: number;
  equipmentTypeId?: number;
}

@Injectable({ providedIn: 'root' })
export class EquipmentService {
  private api = inject(ApiService);

  list(filter?: EquipmentListFilter): Observable<Equipment[]> {
    return this.api.get<Equipment[]>('/equipment', filter as Record<string, number | undefined>);
  }

  create(request: CreateEquipmentRequest): Observable<Equipment> {
    return this.api.post<Equipment>('/equipment', request);
  }

  update(equipmentId: number, request: UpdateEquipmentRequest): Observable<Equipment> {
    return this.api.put<Equipment>(`/equipment/${equipmentId}`, request);
  }

  deactivate(equipmentId: number): Observable<unknown> {
    return this.api.delete(`/equipment/${equipmentId}`);
  }
}
