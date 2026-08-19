import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateUserRequest, UpdateUserRequest, UserDetail, UserListItem } from '../models/user.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = inject(ApiService);

  list(): Observable<UserListItem[]> {
    return this.api.get<UserListItem[]>('/users');
  }

  getById(tnbId: string): Observable<UserDetail> {
    return this.api.get<UserDetail>(`/users/${tnbId}`);
  }

  create(request: CreateUserRequest): Observable<UserDetail> {
    return this.api.post<UserDetail>('/users', request);
  }

  update(tnbId: string, request: UpdateUserRequest): Observable<UserDetail> {
    return this.api.put<UserDetail>(`/users/${tnbId}`, request);
  }
}
