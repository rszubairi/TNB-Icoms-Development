import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { OutageScheduleWindow } from '../models/outage-schedule-window.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class OutageScheduleWindowService {
  private api = inject(ApiService);

  list(): Observable<OutageScheduleWindow[]> {
    return this.api.get<OutageScheduleWindow[]>('/outage-schedule-windows');
  }

  save(windows: OutageScheduleWindow[]): Observable<OutageScheduleWindow[]> {
    return this.api.post<OutageScheduleWindow[]>('/outage-schedule-windows', { windows });
  }
}
