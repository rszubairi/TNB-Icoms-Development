import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ReportFilter, SaveReportFilterRequest, SavedReportFilter } from '../models/report.model';
import { OutageListItem } from '../models/outage.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private api = inject(ApiService);
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  generate(filter: ReportFilter): Observable<OutageListItem[]> {
    return this.api.post<OutageListItem[]>('/reports/generate', filter);
  }

  exportExcel(filter: ReportFilter): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/reports/export/excel`, filter, { responseType: 'blob' });
  }

  exportPdf(filter: ReportFilter): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/reports/export/pdf`, filter, { responseType: 'blob' });
  }

  listFavourites(): Observable<SavedReportFilter[]> {
    return this.api.get<SavedReportFilter[]>('/reports/favourites');
  }

  saveFavourite(request: SaveReportFilterRequest): Observable<SavedReportFilter> {
    return this.api.post<SavedReportFilter>('/reports/favourites', request);
  }

  deleteFavourite(id: number): Observable<unknown> {
    return this.api.delete(`/reports/favourites/${id}`);
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
