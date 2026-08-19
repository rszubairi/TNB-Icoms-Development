import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CommissioningMemoDetail,
  CommissioningMemoListItem,
  CreateCommissioningMemoRequest,
  MemoStageReviewRequest,
  SetCommissioningResultRequest
} from '../models/commissioning-memo.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class CommissioningMemoService {
  private api = inject(ApiService);
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  list(outageId?: number, status?: string): Observable<CommissioningMemoListItem[]> {
    return this.api.get<CommissioningMemoListItem[]>('/commissioning-memos', { outageId, status });
  }

  getById(id: number): Observable<CommissioningMemoDetail> {
    return this.api.get<CommissioningMemoDetail>(`/commissioning-memos/${id}`);
  }

  create(request: CreateCommissioningMemoRequest): Observable<CommissioningMemoDetail> {
    return this.api.post<CommissioningMemoDetail>('/commissioning-memos', request);
  }

  downloadCoverPage(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/commissioning-memos/${id}/cover-page.pdf`, { responseType: 'blob' });
  }

  engineerPicReview(id: number, request: MemoStageReviewRequest): Observable<CommissioningMemoDetail> {
    return this.api.put<CommissioningMemoDetail>(`/commissioning-memos/${id}/engineer-pic-review`, request);
  }

  seReview(id: number, request: MemoStageReviewRequest): Observable<CommissioningMemoDetail> {
    return this.api.put<CommissioningMemoDetail>(`/commissioning-memos/${id}/se-review`, request);
  }

  dceReview(id: number, request: MemoStageReviewRequest): Observable<CommissioningMemoDetail> {
    return this.api.put<CommissioningMemoDetail>(`/commissioning-memos/${id}/dce-review`, request);
  }

  ceGnmReview(id: number, request: MemoStageReviewRequest): Observable<CommissioningMemoDetail> {
    return this.api.put<CommissioningMemoDetail>(`/commissioning-memos/${id}/ce-gnm-review`, request);
  }

  finalSignOff(id: number, request: MemoStageReviewRequest): Observable<CommissioningMemoDetail> {
    return this.api.put<CommissioningMemoDetail>(`/commissioning-memos/${id}/final-sign-off`, request);
  }

  setCommissioningResult(id: number, request: SetCommissioningResultRequest): Observable<CommissioningMemoDetail> {
    return this.api.put<CommissioningMemoDetail>(`/commissioning-memos/${id}/commissioning-result`, request);
  }

  triggerDownload(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    anchor.click();
    window.URL.revokeObjectURL(url);
  }
}
