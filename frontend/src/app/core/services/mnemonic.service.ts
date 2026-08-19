import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MnemonicDocument } from '../models/mnemonic-document.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class MnemonicService {
  private api = inject(ApiService);
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  list(): Observable<MnemonicDocument[]> {
    return this.api.get<MnemonicDocument[]>('/mnemonic');
  }

  upload(file: File): Observable<MnemonicDocument> {
    const formData = new FormData();
    formData.append('file', file);
    return this.api.post<MnemonicDocument>('/mnemonic', formData);
  }

  downloadCurrent(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/mnemonic/current/download`, { responseType: 'blob' });
  }

  download(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/mnemonic/${id}/download`, { responseType: 'blob' });
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
