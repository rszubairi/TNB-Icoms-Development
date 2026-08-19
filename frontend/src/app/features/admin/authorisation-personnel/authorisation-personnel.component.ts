import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthorisationPersonnelService } from '../../../core/services/authorisation-personnel.service';
import { ZoneService } from '../../../core/services/zone.service';
import { AuthorisationPersonnel } from '../../../core/models/authorisation-personnel.model';
import { Zone } from '../../../core/models/zone.model';

interface Draft {
  authorisationPersonnelId: number | null;
  fullName: string;
  email: string;
  staffId: string;
  zoneId: number | null;
  designation: string;
  isActive: boolean;
}

const emptyDraft: Draft = {
  authorisationPersonnelId: null,
  fullName: '',
  email: '',
  staffId: '',
  zoneId: null,
  designation: '',
  isActive: true
};

@Component({
  selector: 'app-authorisation-personnel',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './authorisation-personnel.component.html',
  styleUrl: './authorisation-personnel.component.css'
})
export class AuthorisationPersonnelComponent {
  private personnelService = inject(AuthorisationPersonnelService);
  private zoneService = inject(ZoneService);

  personnel = signal<AuthorisationPersonnel[]>([]);
  zones = signal<Zone[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  draft = signal<Draft>({ ...emptyDraft });
  formError = signal<string | null>(null);
  saving = signal(false);
  rowBusyId = signal<number | null>(null);

  isEditMode = computed(() => this.draft().authorisationPersonnelId != null);

  constructor() {
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.personnelService.list().subscribe({
      next: (personnel) => {
        this.personnel.set(personnel);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load authorisation personnel. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  startCreate(): void {
    this.formError.set(null);
    this.draft.set({ ...emptyDraft });
    this.showForm.set(true);
  }

  startEdit(person: AuthorisationPersonnel): void {
    this.formError.set(null);
    this.draft.set({
      authorisationPersonnelId: person.authorisationPersonnelId,
      fullName: person.fullName,
      email: person.email,
      staffId: person.staffId ?? '',
      zoneId: person.zoneId,
      designation: person.designation ?? '',
      isActive: person.isActive
    });
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.formError.set(null);
  }

  updateField<K extends keyof Draft>(field: K, value: Draft[K]): void {
    this.draft.update((d) => ({ ...d, [field]: value }));
  }

  save(): void {
    const draft = this.draft();
    if (!draft.fullName.trim() || !draft.email.trim() || !draft.zoneId) {
      this.formError.set('Name, email, and zone are all required.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);

    const request = {
      fullName: draft.fullName.trim(),
      email: draft.email.trim(),
      staffId: draft.staffId.trim() || null,
      zoneId: draft.zoneId,
      designation: draft.designation.trim() || null,
      isActive: draft.isActive
    };

    const request$ = draft.authorisationPersonnelId
      ? this.personnelService.update(draft.authorisationPersonnelId, request)
      : this.personnelService.create(request);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.cancelForm();
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to save.');
      }
    });
  }

  deactivate(person: AuthorisationPersonnel): void {
    if (!confirm(`Remove "${person.fullName}" from the authorisation list?`)) return;
    this.rowBusyId.set(person.authorisationPersonnelId);
    this.personnelService.deactivate(person.authorisationPersonnelId).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to remove.');
      }
    });
  }
}
