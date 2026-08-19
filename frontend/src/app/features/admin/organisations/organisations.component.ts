import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrganisationService } from '../../../core/services/organisation.service';
import { StationService } from '../../../core/services/station.service';
import { ZoneService } from '../../../core/services/zone.service';
import { Organisation } from '../../../core/models/organisation.model';
import { Station } from '../../../core/models/station.model';
import { Zone } from '../../../core/models/zone.model';

interface OrgDraft {
  organisationId: number | null;
  organisationName: string;
  organisationCode: string;
  isGcu: boolean;
  isActive: boolean;
}

interface StationDraft {
  stationId: number | null;
  stationName: string;
  stationAbbr: string;
  orgId: number | null;
  isActive: boolean;
}

const emptyOrgDraft: OrgDraft = { organisationId: null, organisationName: '', organisationCode: '', isGcu: false, isActive: true };
const emptyStationDraft: StationDraft = { stationId: null, stationName: '', stationAbbr: '', orgId: null, isActive: true };

@Component({
  selector: 'app-organisations',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './organisations.component.html',
  styleUrl: './organisations.component.css'
})
export class OrganisationsComponent {
  private organisationService = inject(OrganisationService);
  private stationService = inject(StationService);
  private zoneService = inject(ZoneService);

  zones = signal<Zone[]>([]);
  selectedZoneId = signal<number | null>(null);

  organisations = signal<Organisation[]>([]);
  stations = signal<Station[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);

  orgDraft = signal<OrgDraft>({ ...emptyOrgDraft });
  orgFormError = signal<string | null>(null);
  orgSaving = signal(false);
  orgBusyId = signal<number | null>(null);

  stationDraft = signal<StationDraft>({ ...emptyStationDraft });
  stationFormError = signal<string | null>(null);
  stationSaving = signal(false);
  stationBusyId = signal<number | null>(null);

  isOrgEditMode = computed(() => this.orgDraft().organisationId != null);
  isStationEditMode = computed(() => this.stationDraft().stationId != null);

  constructor() {
    this.zoneService.list().subscribe({
      next: (zones) => {
        this.zones.set(zones);
        if (zones.length > 0) {
          this.selectedZoneId.set(zones[0].zoneId);
          this.loadZoneData();
        } else {
          this.loading.set(false);
        }
      },
      error: () => {
        this.errorMessage.set('Unable to load zones. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  onZoneChange(zoneId: string): void {
    this.selectedZoneId.set(zoneId ? Number(zoneId) : null);
    this.resetOrgForm();
    this.resetStationForm();
    this.loadZoneData();
  }

  private loadZoneData(): void {
    const zoneId = this.selectedZoneId();
    if (!zoneId) return;

    this.loading.set(true);
    this.errorMessage.set(null);

    this.organisationService.list(zoneId).subscribe({
      next: (orgs) => {
        this.organisations.set(orgs);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load organisations.');
        this.loading.set(false);
      }
    });

    this.stationService.list(zoneId).subscribe({
      next: (stations) => this.stations.set(stations),
      error: () => this.errorMessage.set('Unable to load stations.')
    });
  }

  // --- Organisations ---

  startCreateOrg(): void {
    this.orgFormError.set(null);
    this.orgDraft.set({ ...emptyOrgDraft });
  }

  startEditOrg(org: Organisation): void {
    this.orgFormError.set(null);
    this.orgDraft.set({
      organisationId: org.organisationId,
      organisationName: org.organisationName,
      organisationCode: org.organisationCode,
      isGcu: org.isGcu,
      isActive: org.isActive
    });
  }

  resetOrgForm(): void {
    this.orgDraft.set({ ...emptyOrgDraft });
    this.orgFormError.set(null);
  }

  updateOrgName(value: string): void {
    this.orgDraft.update((d) => ({ ...d, organisationName: value }));
  }

  updateOrgCode(value: string): void {
    this.orgDraft.update((d) => ({ ...d, organisationCode: value }));
  }

  updateOrgIsGcu(value: boolean): void {
    this.orgDraft.update((d) => ({ ...d, isGcu: value }));
  }

  updateOrgIsActive(value: boolean): void {
    this.orgDraft.update((d) => ({ ...d, isActive: value }));
  }

  saveOrg(): void {
    const draft = this.orgDraft();
    const zoneId = this.selectedZoneId();
    if (!zoneId) return;

    if (!draft.organisationName.trim() || !draft.organisationCode.trim()) {
      this.orgFormError.set('Name and abbreviation are required.');
      return;
    }

    this.orgFormError.set(null);
    this.orgSaving.set(true);

    const request$ = draft.organisationId
      ? this.organisationService.update(draft.organisationId, {
          organisationName: draft.organisationName.trim(),
          organisationCode: draft.organisationCode.trim(),
          isGcu: draft.isGcu,
          isActive: draft.isActive
        })
      : this.organisationService.create({
          organisationName: draft.organisationName.trim(),
          organisationCode: draft.organisationCode.trim(),
          zoneId,
          isGcu: draft.isGcu
        });

    request$.subscribe({
      next: () => {
        this.orgSaving.set(false);
        this.resetOrgForm();
        this.loadZoneData();
      },
      error: (err) => {
        this.orgSaving.set(false);
        this.orgFormError.set(err?.error?.error ?? err?.message ?? 'Unable to save organisation.');
      }
    });
  }

  deactivateOrg(org: Organisation): void {
    if (!confirm(`Deactivate "${org.organisationName}"? It must have no active stations or users first.`)) return;
    this.orgBusyId.set(org.organisationId);
    this.organisationService.deactivate(org.organisationId).subscribe({
      next: () => {
        this.orgBusyId.set(null);
        this.loadZoneData();
      },
      error: (err) => {
        this.orgBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate organisation.');
      }
    });
  }

  // --- Stations ---

  startCreateStation(): void {
    this.stationFormError.set(null);
    this.stationDraft.set({ ...emptyStationDraft });
  }

  startEditStation(station: Station): void {
    this.stationFormError.set(null);
    this.stationDraft.set({
      stationId: station.stationId,
      stationName: station.stationName,
      stationAbbr: station.stationAbbr,
      orgId: station.orgId,
      isActive: station.isActive
    });
  }

  resetStationForm(): void {
    this.stationDraft.set({ ...emptyStationDraft });
    this.stationFormError.set(null);
  }

  updateStationName(value: string): void {
    this.stationDraft.update((d) => ({ ...d, stationName: value }));
  }

  updateStationAbbr(value: string): void {
    this.stationDraft.update((d) => ({ ...d, stationAbbr: value }));
  }

  updateStationOrgId(value: string): void {
    this.stationDraft.update((d) => ({ ...d, orgId: value ? Number(value) : null }));
  }

  updateStationIsActive(value: boolean): void {
    this.stationDraft.update((d) => ({ ...d, isActive: value }));
  }

  saveStation(): void {
    const draft = this.stationDraft();
    const zoneId = this.selectedZoneId();
    if (!zoneId) return;

    if (!draft.stationName.trim() || !draft.stationAbbr.trim() || !draft.orgId) {
      this.stationFormError.set('Name, abbreviation, and organisation are required.');
      return;
    }

    this.stationFormError.set(null);
    this.stationSaving.set(true);

    const request$ = draft.stationId
      ? this.stationService.update(draft.stationId, {
          stationName: draft.stationName.trim(),
          stationAbbr: draft.stationAbbr.trim(),
          orgId: draft.orgId,
          isActive: draft.isActive
        })
      : this.stationService.create({
          stationName: draft.stationName.trim(),
          stationAbbr: draft.stationAbbr.trim(),
          zoneId,
          orgId: draft.orgId
        });

    request$.subscribe({
      next: () => {
        this.stationSaving.set(false);
        this.resetStationForm();
        this.loadZoneData();
      },
      error: (err) => {
        this.stationSaving.set(false);
        this.stationFormError.set(err?.error?.error ?? err?.message ?? 'Unable to save station.');
      }
    });
  }

  deactivateStation(station: Station): void {
    if (!confirm(`Deactivate "${station.stationName}"? It must have no active equipment first.`)) return;
    this.stationBusyId.set(station.stationId);
    this.stationService.deactivate(station.stationId).subscribe({
      next: () => {
        this.stationBusyId.set(null);
        this.loadZoneData();
      },
      error: (err) => {
        this.stationBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate station.');
      }
    });
  }
}
