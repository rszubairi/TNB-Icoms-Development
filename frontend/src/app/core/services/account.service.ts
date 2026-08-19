import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountProfile, ChangePasswordRequest, UpdateAccountProfileRequest } from '../models/account.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private api = inject(ApiService);

  getMe(): Observable<AccountProfile> {
    return this.api.get<AccountProfile>('/account/me');
  }

  updateMe(request: UpdateAccountProfileRequest): Observable<AccountProfile> {
    return this.api.put<AccountProfile>('/account/me', request);
  }

  changePassword(request: ChangePasswordRequest): Observable<unknown> {
    return this.api.post('/account/me/password', request);
  }
}
