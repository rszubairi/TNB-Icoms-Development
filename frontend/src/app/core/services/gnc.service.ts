import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CancelOutageRequest,
  CompleteAuthorisationRequest,
  ExtendAuthorisationRequest,
  ForcedOutageRequest,
  GncOutageListItem,
  NotTakenRequest,
  TakeActiveRequest
} from '../models/gnc.model';
import { OutageDetail } from '../models/outage.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class GncService {
  private api = inject(ApiService);

  listScheduled(zoneId?: number): Observable<GncOutageListItem[]> {
    return this.api.get<GncOutageListItem[]>('/gnc/scheduled', { zoneId });
  }

  listActive(zoneId?: number): Observable<GncOutageListItem[]> {
    return this.api.get<GncOutageListItem[]>('/gnc/active', { zoneId });
  }

  listAuthorisationInForce(zoneId?: number): Observable<GncOutageListItem[]> {
    return this.api.get<GncOutageListItem[]>('/gnc/authorisation-in-force', { zoneId });
  }

  takeActive(outageId: number, request: TakeActiveRequest): Observable<unknown> {
    return this.api.post(`/gnc/outages/${outageId}/take-active`, request);
  }

  complete(outageId: number, request: CompleteAuthorisationRequest): Observable<unknown> {
    return this.api.post(`/gnc/outages/${outageId}/complete`, request);
  }

  extend(outageId: number, request: ExtendAuthorisationRequest): Observable<unknown> {
    return this.api.post(`/gnc/outages/${outageId}/extend`, request);
  }

  close(outageId: number): Observable<unknown> {
    return this.api.post(`/gnc/outages/${outageId}/close`, {});
  }

  notTaken(outageId: number, request: NotTakenRequest): Observable<unknown> {
    return this.api.post(`/gnc/outages/${outageId}/not-taken`, request);
  }

  cancel(outageId: number, request: CancelOutageRequest): Observable<unknown> {
    return this.api.post(`/gnc/outages/${outageId}/cancel`, request);
  }

  createForcedOutage(request: ForcedOutageRequest): Observable<OutageDetail> {
    return this.api.post<OutageDetail>('/gnc/forced-outages', request);
  }
}
