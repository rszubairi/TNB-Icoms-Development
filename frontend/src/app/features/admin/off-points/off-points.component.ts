import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EquipmentService } from '../../../core/services/equipment.service';
import { ZoneService } from '../../../core/services/zone.service';
import { StationService } from '../../../core/services/station.service';
import { Equipment } from '../../../core/models/equipment.model';
import { Zone } from '../../../core/models/zone.model';
import { Station } from '../../../core/models/station.model';

type OffPointTab = 'permanent' | 'temporary';

interface CreateDraft {
  zoneId: number | null;
  stationId: number | null;
  equipmentId: number | null;
  remark: string;
}

const emptyDraft: CreateDraft = { zoneId: null, stationId: null, equipmentId: null, remark: '' };

@Component({
  selector: 'app-off-points',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './off-points.component.html',
  styleUrl: './off-points.component.css'
})
export class OffPointsComponent {
  private equipmentService = inject(EquipmentService);
  private zoneService = inject(ZoneService);
  private stationService = inject(StationService);

  activeTab = signal<OffPointTab>('permanent');
  permanentOffPoints = signal<Equipment[]>([]);
  regionFilter = signal<number | null>(null);

  zones = signal<Zone[]>([]);
  allStations = signal<Station[]>([]);
  eligibleEquipment = signal<Equipment[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);
  rowBusyId = signal<number | null>(null);

  showCreateForm = signal(false);
  createDraft = signal<CreateDraft>({ ...emptyDraft });
  createFormError = signal<string | null>(null);
  creating = signal(false);

  filteredOffPoints = computed(() => {
    const region = this.regionFilter();
    const items = this.permanentOffPoints();
    return region ? items.filter((e) => e.zoneId === region) : items;
  });

  stationsForDraftZone = computed(() => {
    const zoneId = this.createDraft().zoneId;
    if (!zoneId) return [];
    return this.allStations().filter((s) => s.zoneId === zoneId);
  });

  constructor() {
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
    this.stationService.list().subscribe({ next: (stations) => this.allStations.set(stations), error: () => {} });
    this.loadPermanentOffPoints();
  }

  private loadPermanentOffPoints(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.equipmentService.list({ isOffPoint: true }).subscribe({
      next: (items) => {
        this.permanentOffPoints.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load off-points. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  setTab(tab: OffPointTab): void {
    this.activeTab.set(tab);
  }

  onRegionFilterChange(value: string): void {
    this.regionFilter.set(value ? Number(value) : null);
  }

  // --- Normalize position toggle ---

  toggleNormalize(equipment: Equipment): void {
    const nextIsOpen = equipment.position !== 1;
    if (!confirm(`Set "${equipment.equipmentName}" to ${nextIsOpen ? 'Open' : 'Closed'}?`)) return;

    this.rowBusyId.set(equipment.equipmentId);
    this.equipmentService
      .update(equipment.equipmentId, {
        name: equipment.shortName,
        mvaRatingId: equipment.mvaRatingId,
        isOpen: nextIsOpen,
        isOffPoint: equipment.isOffPoint,
        offPointRemark: equipment.offPointRemark,
        isActive: equipment.isActive
      })
      .subscribe({
        next: () => {
          this.rowBusyId.set(null);
          this.loadPermanentOffPoints();
        },
        error: (err) => {
          this.rowBusyId.set(null);
          this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to update equipment position.');
        }
      });
  }

  removeOffPoint(equipment: Equipment): void {
    if (!confirm(`Remove "${equipment.equipmentName}" from the Permanent Off-Points list? It will be normalized to Closed.`)) return;

    this.rowBusyId.set(equipment.equipmentId);
    this.equipmentService
      .update(equipment.equipmentId, {
        name: equipment.shortName,
        mvaRatingId: equipment.mvaRatingId,
        isOpen: false,
        isOffPoint: false,
        offPointRemark: null,
        isActive: equipment.isActive
      })
      .subscribe({
        next: () => {
          this.rowBusyId.set(null);
          this.loadPermanentOffPoints();
        },
        error: (err) => {
          this.rowBusyId.set(null);
          this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to remove off-point.');
        }
      });
  }

  // --- Create new off-point (mark existing equipment) ---

  openCreateForm(): void {
    this.createFormError.set(null);
    this.createDraft.set({ ...emptyDraft });
    this.eligibleEquipment.set([]);
    this.showCreateForm.set(true);
  }

  closeCreateForm(): void {
    this.showCreateForm.set(false);
    this.createDraft.set({ ...emptyDraft });
    this.createFormError.set(null);
  }

  onDraftZoneChange(value: string): void {
    this.createDraft.update((d) => ({ ...d, zoneId: value ? Number(value) : null, stationId: null, equipmentId: null }));
    this.eligibleEquipment.set([]);
  }

  onDraftStationChange(value: string): void {
    const stationId = value ? Number(value) : null;
    this.createDraft.update((d) => ({ ...d, stationId, equipmentId: null }));
    this.eligibleEquipment.set([]);
    if (stationId) {
      this.equipmentService.list({ stationId }).subscribe({
        next: (items) => this.eligibleEquipment.set(items.filter((e) => e.isActive && !e.isOffPoint)),
        error: () => {}
      });
    }
  }

  onDraftEquipmentChange(value: string): void {
    this.createDraft.update((d) => ({ ...d, equipmentId: value ? Number(value) : null }));
  }

  updateDraftRemark(value: string): void {
    this.createDraft.update((d) => ({ ...d, remark: value }));
  }

  submitCreate(): void {
    const draft = this.createDraft();
    if (!draft.equipmentId) {
      this.createFormError.set('Select the equipment to mark as an off-point.');
      return;
    }
    if (!draft.remark.trim()) {
      this.createFormError.set('A remark is required.');
      return;
    }

    const equipment = this.eligibleEquipment().find((e) => e.equipmentId === draft.equipmentId);
    if (!equipment) return;

    this.createFormError.set(null);
    this.creating.set(true);

    this.equipmentService
      .update(equipment.equipmentId, {
        name: equipment.shortName,
        mvaRatingId: equipment.mvaRatingId,
        isOpen: true,
        isOffPoint: true,
        offPointRemark: draft.remark.trim(),
        isActive: equipment.isActive
      })
      .subscribe({
        next: () => {
          this.creating.set(false);
          this.closeCreateForm();
          this.loadPermanentOffPoints();
        },
        error: (err) => {
          this.creating.set(false);
          this.createFormError.set(err?.error?.error ?? err?.message ?? 'Unable to create off-point.');
        }
      });
  }
}
