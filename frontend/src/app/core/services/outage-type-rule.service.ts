import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { OutageTypeRule, SaveOutageTypeRuleRequest } from '../models/outage-type-rule.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class OutageTypeRuleService {
  private api = inject(ApiService);

  list(): Observable<OutageTypeRule[]> {
    return this.api.get<OutageTypeRule[]>('/outage-type-rules');
  }

  create(request: SaveOutageTypeRuleRequest): Observable<OutageTypeRule> {
    return this.api.post<OutageTypeRule>('/outage-type-rules', request);
  }

  update(ruleId: number, request: SaveOutageTypeRuleRequest): Observable<OutageTypeRule> {
    return this.api.put<OutageTypeRule>(`/outage-type-rules/${ruleId}`, request);
  }

  deactivate(ruleId: number): Observable<unknown> {
    return this.api.delete(`/outage-type-rules/${ruleId}`);
  }
}
