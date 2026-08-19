import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorisationPersonnel, SaveAuthorisationPersonnelRequest } from '../models/authorisation-personnel.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class AuthorisationPersonnelService {
  private api = inject(ApiService);

  list(zoneId?: number): Observable<AuthorisationPersonnel[]> {
    return this.api.get<AuthorisationPersonnel[]>('/authorisation-personnel', zoneId ? { zoneId } : undefined);
  }

  create(request: SaveAuthorisationPersonnelRequest): Observable<AuthorisationPersonnel> {
    return this.api.post<AuthorisationPersonnel>('/authorisation-personnel', request);
  }

  update(personnelId: number, request: SaveAuthorisationPersonnelRequest): Observable<AuthorisationPersonnel> {
    return this.api.put<AuthorisationPersonnel>(`/authorisation-personnel/${personnelId}`, request);
  }

  deactivate(personnelId: number): Observable<unknown> {
    return this.api.delete(`/authorisation-personnel/${personnelId}`);
  }
}
