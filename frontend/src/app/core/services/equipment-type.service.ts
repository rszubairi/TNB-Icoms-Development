import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateEquipmentTypeRequest, EquipmentType, UpdateEquipmentTypeRequest } from '../models/equipment-type.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class EquipmentTypeService {
  private api = inject(ApiService);

  list(voltageLevelId?: number): Observable<EquipmentType[]> {
    return this.api.get<EquipmentType[]>('/equipment-types', voltageLevelId ? { voltageLevelId } : undefined);
  }

  create(request: CreateEquipmentTypeRequest): Observable<EquipmentType> {
    return this.api.post<EquipmentType>('/equipment-types', request);
  }

  update(equipmentTypeId: number, request: UpdateEquipmentTypeRequest): Observable<EquipmentType> {
    return this.api.put<EquipmentType>(`/equipment-types/${equipmentTypeId}`, request);
  }

  deactivate(equipmentTypeId: number): Observable<unknown> {
    return this.api.delete(`/equipment-types/${equipmentTypeId}`);
  }
}
