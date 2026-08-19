import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateRoleTransferRequest, RoleTransferRequest } from '../models/role-transfer-request.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class RoleTransferRequestService {
  private api = inject(ApiService);

  list(): Observable<RoleTransferRequest[]> {
    return this.api.get<RoleTransferRequest[]>('/role-transfer-requests');
  }

  create(request: CreateRoleTransferRequest): Observable<RoleTransferRequest> {
    return this.api.post<RoleTransferRequest>('/role-transfer-requests', request);
  }

  approve(id: number): Observable<void> {
    return this.api.post<void>(`/role-transfer-requests/${id}/approve`, {});
  }

  reject(id: number): Observable<void> {
    return this.api.post<void>(`/role-transfer-requests/${id}/reject`, {});
  }
}
