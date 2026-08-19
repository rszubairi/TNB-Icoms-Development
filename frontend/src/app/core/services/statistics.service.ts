import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { StatisticsDashboard } from '../models/statistics.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  private api = inject(ApiService);

  getDashboard(year: number, month: number | null): Observable<StatisticsDashboard> {
    return this.api.get<StatisticsDashboard>('/statistics/dashboard', { year, month: month ?? undefined });
  }
}
