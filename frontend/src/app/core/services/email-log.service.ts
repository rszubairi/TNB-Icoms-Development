import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { EmailLog } from '../models/email-log.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class EmailLogService {
  private api = inject(ApiService);

  list(status?: string, templateCode?: string, toAddress?: string, dateStart?: string, dateEnd?: string): Observable<EmailLog[]> {
    return this.api.get<EmailLog[]>('/email-logs', { status, templateCode, toAddress, dateStart, dateEnd });
  }
}
