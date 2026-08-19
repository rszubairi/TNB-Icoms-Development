import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AddEntryRequest,
  HandoverEntry,
  HandoverShift,
  HandoverShiftSummary,
  UpdateShiftControlRequest
} from '../models/handover.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class HandoverService {
  private api = inject(ApiService);

  listCategories(): Observable<{ categories: string[] }> {
    return this.api.get<{ categories: string[] }>('/handover/categories');
  }

  getOrCreateShift(shiftDate: string, shiftType: string, zoneId: number): Observable<HandoverShift> {
    return this.api.get<HandoverShift>('/handover/shift', { shiftDate, shiftType, zoneId });
  }

  listShifts(zoneId: number, dateStart?: string, dateEnd?: string): Observable<HandoverShiftSummary[]> {
    return this.api.get<HandoverShiftSummary[]>('/handover/shifts', { zoneId, dateStart, dateEnd });
  }

  updateShiftControl(shiftId: number, request: UpdateShiftControlRequest): Observable<unknown> {
    return this.api.put(`/handover/shifts/${shiftId}/control`, request);
  }

  addEntry(shiftId: number, request: AddEntryRequest): Observable<HandoverEntry> {
    return this.api.post<HandoverEntry>(`/handover/shifts/${shiftId}/entries`, request);
  }

  deleteEntry(entryId: number): Observable<unknown> {
    return this.api.delete(`/handover/entries/${entryId}`);
  }

  passHandover(shiftId: number): Observable<HandoverShift> {
    return this.api.post<HandoverShift>(`/handover/shifts/${shiftId}/pass`, {});
  }
}
