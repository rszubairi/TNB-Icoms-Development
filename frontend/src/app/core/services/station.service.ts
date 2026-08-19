import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateStationRequest, Station, UpdateStationRequest } from '../models/station.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class StationService {
  private api = inject(ApiService);

  list(zoneId?: number, orgId?: number): Observable<Station[]> {
    const params: Record<string, number> = {};
    if (zoneId) params['zoneId'] = zoneId;
    if (orgId) params['orgId'] = orgId;
    return this.api.get<Station[]>('/stations', params);
  }

  create(request: CreateStationRequest): Observable<Station> {
    return this.api.post<Station>('/stations', request);
  }

  update(stationId: number, request: UpdateStationRequest): Observable<Station> {
    return this.api.put<Station>(`/stations/${stationId}`, request);
  }

  deactivate(stationId: number): Observable<unknown> {
    return this.api.delete(`/stations/${stationId}`);
  }
}
