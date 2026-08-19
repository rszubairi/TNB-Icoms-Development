import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Zone } from '../models/zone.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ZoneService {
  private api = inject(ApiService);

  list(): Observable<Zone[]> {
    return this.api.get<Zone[]>('/zones');
  }
}
