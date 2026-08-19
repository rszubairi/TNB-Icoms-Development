import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateOrganisationRequest, Organisation, UpdateOrganisationRequest } from '../models/organisation.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class OrganisationService {
  private api = inject(ApiService);

  list(zoneId?: number): Observable<Organisation[]> {
    return this.api.get<Organisation[]>('/organisations', zoneId ? { zoneId } : undefined);
  }

  create(request: CreateOrganisationRequest): Observable<Organisation> {
    return this.api.post<Organisation>('/organisations', request);
  }

  update(organisationId: number, request: UpdateOrganisationRequest): Observable<Organisation> {
    return this.api.put<Organisation>(`/organisations/${organisationId}`, request);
  }

  deactivate(organisationId: number): Observable<unknown> {
    return this.api.delete(`/organisations/${organisationId}`);
  }
}
