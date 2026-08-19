import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateProjectRequest, Project } from '../models/project.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private api = inject(ApiService);

  list(isActive?: boolean): Observable<Project[]> {
    return this.api.get<Project[]>('/projects', isActive === undefined ? undefined : { isActive });
  }

  create(request: CreateProjectRequest): Observable<Project> {
    return this.api.post<Project>('/projects', request);
  }

  setStatus(projectId: number, isActive: boolean): Observable<Project> {
    return this.api.post<Project>(`/projects/${projectId}/status`, { isActive });
  }
}
