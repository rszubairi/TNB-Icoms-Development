import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ChangeRequestBatch, CreateChangeRequestBatch } from '../models/change-request.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ChangeRequestService {
  private api = inject(ApiService);

  listForOutage(outageId: number): Observable<ChangeRequestBatch[]> {
    return this.api.get<ChangeRequestBatch[]>(`/change-requests/by-outage/${outageId}`);
  }

  listPending(): Observable<ChangeRequestBatch[]> {
    return this.api.get<ChangeRequestBatch[]>('/change-requests/pending');
  }

  create(request: CreateChangeRequestBatch): Observable<ChangeRequestBatch> {
    return this.api.post<ChangeRequestBatch>('/change-requests', request);
  }

  approve(batchId: string): Observable<unknown> {
    return this.api.post(`/change-requests/${batchId}/approve`, {});
  }

  reject(batchId: string, comment: string): Observable<unknown> {
    return this.api.post(`/change-requests/${batchId}/reject`, { comment });
  }
}
