import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../../core/services/project.service';
import { ZoneService } from '../../../core/services/zone.service';
import { Project } from '../../../core/models/project.model';
import { Zone } from '../../../core/models/zone.model';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.css'
})
export class ProjectsComponent {
  private projectService = inject(ProjectService);
  private zoneService = inject(ZoneService);

  projects = signal<Project[]>([]);
  zones = signal<Zone[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  tpCode = signal('');
  projectSuffix = signal('');
  zoneId = signal<number | null>(null);
  formError = signal<string | null>(null);
  saving = signal(false);
  rowBusyId = signal<number | null>(null);

  openProjects = computed(() => this.projects().filter((p) => p.isActive));
  closedProjects = computed(() => this.projects().filter((p) => !p.isActive));

  constructor() {
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.projectService.list().subscribe({
      next: (projects) => {
        this.projects.set(projects);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load projects. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  openCreateForm(): void {
    this.formError.set(null);
    this.tpCode.set('');
    this.projectSuffix.set('');
    this.zoneId.set(null);
    this.showForm.set(true);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.formError.set(null);
  }

  create(): void {
    if (!this.tpCode().trim() || !this.projectSuffix().trim()) {
      this.formError.set('TP Code and Name are both required.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);

    this.projectService
      .create({
        tpCode: this.tpCode().trim(),
        projectSuffix: this.projectSuffix().trim(),
        zoneId: this.zoneId()
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.closeForm();
          this.load();
        },
        error: (err) => {
          this.saving.set(false);
          this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to create project.');
        }
      });
  }

  markComplete(project: Project): void {
    const warning =
      project.openOutageCount > 0
        ? `"${project.projectName}" still has ${project.openOutageCount} open outage(s). They will continue as normal, but no new outages may be assigned to this project. Mark it Complete anyway?`
        : `Mark "${project.projectName}" as Complete? It will no longer be available in the Project dropdown.`;

    if (!confirm(warning)) return;

    this.rowBusyId.set(project.projectId);
    this.projectService.setStatus(project.projectId, false).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to update project status.');
      }
    });
  }

  reopen(project: Project): void {
    if (!confirm(`Re-open "${project.projectName}"? It will become available in the Project dropdown again.`)) return;

    this.rowBusyId.set(project.projectId);
    this.projectService.setStatus(project.projectId, true).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to update project status.');
      }
    });
  }
}
