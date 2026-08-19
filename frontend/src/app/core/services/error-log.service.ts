import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorLog } from '../models/error-log.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ErrorLogService {
  private api = inject(ApiService);

  list(source?: string, severity?: string, dateStart?: string, dateEnd?: string): Observable<ErrorLog[]> {
    return this.api.get<ErrorLog[]>('/errors', { source, severity, dateStart, dateEnd });
  }
}
