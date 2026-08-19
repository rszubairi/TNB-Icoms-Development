import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateRoleRequest, PermissionModulesResponse, Role, RoleDetail, UpdateRoleRequest } from '../models/role.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private api = inject(ApiService);

  list(): Observable<Role[]> {
    return this.api.get<Role[]>('/roles');
  }

  getById(roleId: number): Observable<RoleDetail> {
    return this.api.get<RoleDetail>(`/roles/${roleId}`);
  }

  listPermissionModules(): Observable<PermissionModulesResponse> {
    return this.api.get<PermissionModulesResponse>('/roles/modules');
  }

  create(request: CreateRoleRequest): Observable<RoleDetail> {
    return this.api.post<RoleDetail>('/roles', request);
  }

  update(roleId: number, request: UpdateRoleRequest): Observable<RoleDetail> {
    return this.api.put<RoleDetail>(`/roles/${roleId}`, request);
  }

  deactivate(roleId: number): Observable<unknown> {
    return this.api.delete(`/roles/${roleId}`);
  }
}
