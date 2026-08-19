import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { EmailTemplate, UpdateEmailTemplateRequest } from '../models/email-template.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class EmailTemplateService {
  private api = inject(ApiService);

  list(): Observable<EmailTemplate[]> {
    return this.api.get<EmailTemplate[]>('/email-templates');
  }

  getByCode(templateCode: string): Observable<EmailTemplate> {
    return this.api.get<EmailTemplate>(`/email-templates/${templateCode}`);
  }

  update(templateCode: string, request: UpdateEmailTemplateRequest): Observable<EmailTemplate> {
    return this.api.put<EmailTemplate>(`/email-templates/${templateCode}`, request);
  }
}
