import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface SystemSetting {
  settingKey: string;
  settingValue: string;
  updatedAt: string;
}

@Injectable({ providedIn: 'root' })
export class ChangeRequestSettingsService {
  private api = inject(ApiService);

  get(): Observable<SystemSetting> {
    return this.api.get<SystemSetting>('/change-request-settings');
  }

  save(days: number): Observable<SystemSetting> {
    return this.api.put<SystemSetting>('/change-request-settings', { settingValue: String(days) });
  }
}
