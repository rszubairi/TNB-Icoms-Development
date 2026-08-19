import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Role } from '../models/role.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private api = inject(ApiService);

  list(): Observable<Role[]> {
    return this.api.get<Role[]>('/roles');
  }
}
