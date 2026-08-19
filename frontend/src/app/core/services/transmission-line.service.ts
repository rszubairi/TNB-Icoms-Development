import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { GeneratedName, TransmissionLine, TransmissionLineRequest } from '../models/transmission-line.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class TransmissionLineService {
  private api = inject(ApiService);

  list(): Observable<TransmissionLine[]> {
    return this.api.get<TransmissionLine[]>('/transmission-lines');
  }

  preview(request: TransmissionLineRequest): Observable<GeneratedName[]> {
    return this.api.post<GeneratedName[]>('/transmission-lines/preview', request);
  }

  create(request: TransmissionLineRequest): Observable<TransmissionLine> {
    return this.api.post<TransmissionLine>('/transmission-lines', request);
  }

  addOwnerZone(lineId: number, zoneId: number): Observable<TransmissionLine> {
    return this.api.post<TransmissionLine>(`/transmission-lines/${lineId}/owner-zones`, { zoneId });
  }

  removeOwnerZone(lineId: number, zoneId: number): Observable<unknown> {
    return this.api.delete(`/transmission-lines/${lineId}/owner-zones/${zoneId}`);
  }

  deactivate(lineId: number): Observable<unknown> {
    return this.api.delete(`/transmission-lines/${lineId}`);
  }
}
