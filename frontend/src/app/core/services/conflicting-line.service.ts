import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ConflictingLine, CreateConflictingLineRequest } from '../models/conflicting-line.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ConflictingLineService {
  private api = inject(ApiService);

  list(): Observable<ConflictingLine[]> {
    return this.api.get<ConflictingLine[]>('/conflicting-lines');
  }

  create(request: CreateConflictingLineRequest): Observable<ConflictingLine> {
    return this.api.post<ConflictingLine>('/conflicting-lines', request);
  }

  deactivate(id: number): Observable<unknown> {
    return this.api.delete(`/conflicting-lines/${id}`);
  }
}
