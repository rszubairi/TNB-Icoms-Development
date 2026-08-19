import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateLinkingLineRequest, LinkingLine } from '../models/linking-line.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class LinkingLineService {
  private api = inject(ApiService);

  list(): Observable<LinkingLine[]> {
    return this.api.get<LinkingLine[]>('/linking-lines');
  }

  create(request: CreateLinkingLineRequest): Observable<LinkingLine> {
    return this.api.post<LinkingLine>('/linking-lines', request);
  }

  deactivate(id: number): Observable<unknown> {
    return this.api.delete(`/linking-lines/${id}`);
  }
}
