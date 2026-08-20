import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Dashboard } from '../models/dashboard.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private api = inject(ApiService);

  get(zoneId?: number): Observable<Dashboard> {
    return this.api.get<Dashboard>('/dashboard', { zoneId });
  }
}
