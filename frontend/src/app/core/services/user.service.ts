import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { CreateUserRequest, PagedResult, UpdateUserRequest, UserDetail, UserListItem } from '../models/user.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = inject(ApiService);

  list(): Observable<UserListItem[]> {
    return this.api
      .get<PagedResult<UserListItem>>('/users')
      .pipe(map((result) => result.items));
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
