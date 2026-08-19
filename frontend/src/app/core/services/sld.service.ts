import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateSldRequest,
  EngineerReviewRequest,
  SldDetail,
  SldListItem,
  StageReviewRequest
} from '../models/sld.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class SldService {
  private api = inject(ApiService);
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  list(stationId?: number, status?: string): Observable<SldListItem[]> {
    return this.api.get<SldListItem[]>('/sld', { stationId, status });
  }

  getById(id: number): Observable<SldDetail> {
    return this.api.get<SldDetail>(`/sld/${id}`);
  }

  create(request: CreateSldRequest): Observable<SldDetail> {
    return this.api.post<SldDetail>('/sld', request);
  }

  uploadDrawing(id: number, file: File): Observable<SldDetail> {
    const formData = new FormData();
    formData.append('file', file);
    return this.api.post<SldDetail>(`/sld/${id}/drawing`, formData);
  }

  downloadDrawing(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/sld/${id}/drawing/download`, { responseType: 'blob' });
  }

  engineerReview(id: number, request: EngineerReviewRequest): Observable<SldDetail> {
    return this.api.put<SldDetail>(`/sld/${id}/engineer-review`, request);
  }

  seReview(id: number, request: StageReviewRequest): Observable<SldDetail> {
    return this.api.put<SldDetail>(`/sld/${id}/se-review`, request);
  }

  dceReview(id: number, request: StageReviewRequest): Observable<SldDetail> {
    return this.api.put<SldDetail>(`/sld/${id}/dce-review`, request);
  }

  requestorApprove(id: number, request: StageReviewRequest): Observable<SldDetail> {
    return this.api.put<SldDetail>(`/sld/${id}/requestor-approve`, request);
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
