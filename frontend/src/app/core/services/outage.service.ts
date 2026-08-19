import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  BulkActionResult,
  CreateOutageRequest,
  OutageDetail,
  OutageListFilter,
  OutageListItem,
  StudyUpdateRequest
} from '../models/outage.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class OutageService {
  private api = inject(ApiService);

  list(filter?: OutageListFilter): Observable<OutageListItem[]> {
    return this.api.get<OutageListItem[]>('/outages', filter as Record<string, string | number | boolean | undefined>);
  }

  getById(outageId: number): Observable<OutageDetail> {
    return this.api.get<OutageDetail>(`/outages/${outageId}`);
  }

  create(request: CreateOutageRequest): Observable<OutageDetail> {
    return this.api.post<OutageDetail>('/outages', request);
  }

  submitDraft(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/submit`, {});
  }

  agree(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/agree`, {});
  }

  disagree(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/disagree`, {});
  }

  confirm(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/confirm`, {});
  }

  reject(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/reject`, {});
  }

  bulkAgree(outageIds: number[]): Observable<BulkActionResult> {
    return this.api.post<BulkActionResult>('/outages/bulk/agree', { outageIds });
  }

  bulkDisagree(outageIds: number[]): Observable<BulkActionResult> {
    return this.api.post<BulkActionResult>('/outages/bulk/disagree', { outageIds });
  }

  bulkConfirm(outageIds: number[]): Observable<BulkActionResult> {
    return this.api.post<BulkActionResult>('/outages/bulk/confirm', { outageIds });
  }

  bulkReject(outageIds: number[]): Observable<BulkActionResult> {
    return this.api.post<BulkActionResult>('/outages/bulk/reject', { outageIds });
  }

  startStudy(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/study/start`, {});
  }

  updateStudy(outageId: number, request: StudyUpdateRequest, notify: boolean): Observable<unknown> {
    return this.api.put(`/outages/${outageId}/study?notify=${notify}`, request);
  }

  setKiv(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/kiv`, {});
  }

  gnmApprove(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/gnm-approve`, {});
  }

  gnmDisapprove(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/gnm-disapprove`, {});
  }

  gnmRevert(outageId: number): Observable<unknown> {
    return this.api.post(`/outages/${outageId}/gnm-revert`, {});
  }

  bulkGnmApprove(outageIds: number[]): Observable<BulkActionResult> {
    return this.api.post<BulkActionResult>('/outages/bulk/gnm-approve', { outageIds });
  }

  bulkGnmDisapprove(outageIds: number[]): Observable<BulkActionResult> {
    return this.api.post<BulkActionResult>('/outages/bulk/gnm-disapprove', { outageIds });
  }
}
