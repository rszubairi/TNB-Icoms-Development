import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ZoneService } from '../../../core/services/zone.service';
import { StationService } from '../../../core/services/station.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { EquipmentService } from '../../../core/services/equipment.service';
import { DropdownValueService } from '../../../core/services/dropdown-value.service';
import { Zone } from '../../../core/models/zone.model';
import { Station } from '../../../core/models/station.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';
import { EquipmentType } from '../../../core/models/equipment-type.model';
import { Equipment } from '../../../core/models/equipment.model';
import { DropdownValue } from '../../../core/models/dropdown-value.model';

interface EquipmentDraft {
  equipmentId: number | null;
  zoneId: number | null;
  stationId: number | null;
  voltageLevelId: number | null;
  equipmentTypeId: number | null;
  name: string;
  mvaRatingId: number | null;
  isOpen: boolean;
  isOffPoint: boolean;
  offPointRemark: string;
  isActive: boolean;
}

const emptyDraft: EquipmentDraft = {
  equipmentId: null,
  zoneId: null,
  stationId: null,
  voltageLevelId: null,
  equipmentTypeId: null,
  name: '',
  mvaRatingId: null,
  isOpen: false,
  isOffPoint: false,
  offPointRemark: '',
  isActive: true
};

@Component({
  selector: 'app-equipment',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './equipment.component.html',
  styleUrl: './equipment.component.css'
})
export class EquipmentComponent {
  private zoneService = inject(ZoneService);
  private stationService = inject(StationService);
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentTypeService = inject(EquipmentTypeService);
  private equipmentService = inject(EquipmentService);
  private dropdownValueService = inject(DropdownValueService);

  zones = signal<Zone[]>([]);
  allStations = signal<Station[]>([]);
  voltageLevels = signal<VoltageLevel[]>([]);
  equipmentTypesForVoltage = signal<EquipmentType[]>([]);
  mvaRatings = signal<DropdownValue[]>([]);
  equipmentList = signal<Equipment[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  draft = signal<EquipmentDraft>({ ...emptyDraft });
  formError = signal<string | null>(null);
  saving = signal(false);
  rowBusyId = signal<number | null>(null);

  isEditMode = computed(() => this.draft().equipmentId != null);

  stationsForZone = computed(() => {
    const zoneId = this.draft().zoneId;
    if (!zoneId) return [];
    return this.allStations().filter((s) => s.zoneId === zoneId);
  });

  constructor() {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
    this.voltageLevelService.list().subscribe({ next: (levels) => this.voltageLevels.set(levels), error: () => {} });
    this.stationService.list().subscribe({ next: (stations) => this.allStations.set(stations), error: () => {} });
    this.dropdownValueService.listByCategory('MvaRating').subscribe({ next: (values) => this.mvaRatings.set(values), error: () => {} });

    this.loadEquipment();
  }

  private loadEquipment(): void {
    this.loading.set(true);
    this.equipmentService.list().subscribe({
      next: (list) => {
        this.equipmentList.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load equipment. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  // --- Cascade handlers ---

  onZoneChange(value: string): void {
    const zoneId = value ? Number(value) : null;
    this.draft.update((d) => ({ ...d, zoneId, stationId: null }));
  }

  onStationChange(value: string): void {
    this.draft.update((d) => ({ ...d, stationId: value ? Number(value) : null }));
  }

  onVoltageChange(value: string): void {
    const voltageLevelId = value ? Number(value) : null;
    this.draft.update((d) => ({ ...d, voltageLevelId, equipmentTypeId: null }));
    this.equipmentTypesForVoltage.set([]);
    if (voltageLevelId) {
      this.equipmentTypeService.list(voltageLevelId).subscribe({
        next: (types) => this.equipmentTypesForVoltage.set(types.filter((t) => t.isActive)),
        error: () => {}
      });
    }
  }

  onEquipmentTypeChange(value: string): void {
    this.draft.update((d) => ({ ...d, equipmentTypeId: value ? Number(value) : null }));
  }

  onMvaChange(value: string): void {
    this.draft.update((d) => ({ ...d, mvaRatingId: value ? Number(value) : null }));
  }

  updateName(value: string): void {
    this.draft.update((d) => ({ ...d, name: value }));
  }

  updateIsOpen(value: boolean): void {
    this.draft.update((d) => ({ ...d, isOpen: value }));
  }

  updateIsOffPoint(value: boolean): void {
    // URS Module 1 §5.2.4: checking Off-Point forces the position to Open.
    this.draft.update((d) => ({ ...d, isOffPoint: value, isOpen: value ? true : d.isOpen }));
  }

  updateOffPointRemark(value: string): void {
    this.draft.update((d) => ({ ...d, offPointRemark: value }));
  }

  updateIsActive(value: boolean): void {
    this.draft.update((d) => ({ ...d, isActive: value }));
  }

  // --- Form lifecycle ---

  startCreate(): void {
    this.formError.set(null);
    this.draft.set({ ...emptyDraft });
    this.equipmentTypesForVoltage.set([]);
    this.showForm.set(true);
  }

  startEdit(equipment: Equipment): void {
    this.formError.set(null);
    this.draft.set({
      equipmentId: equipment.equipmentId,
      zoneId: equipment.zoneId,
      stationId: equipment.stationId,
      voltageLevelId: equipment.voltageLevelId,
      equipmentTypeId: equipment.equipmentTypeId,
      name: equipment.shortName,
      mvaRatingId: equipment.mvaRatingId,
      isOpen: equipment.position === 1,
      isOffPoint: equipment.isOffPoint,
      offPointRemark: equipment.offPointRemark ?? '',
      isActive: equipment.isActive
    });
    this.equipmentTypeService.list(equipment.voltageLevelId).subscribe({
      next: (types) => this.equipmentTypesForVoltage.set(types.filter((t) => t.isActive || t.equipmentTypeId === equipment.equipmentTypeId)),
      error: () => {}
    });
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.draft.set({ ...emptyDraft });
    this.formError.set(null);
  }

  save(): void {
    const draft = this.draft();

    if (!draft.name.trim()) {
      this.formError.set('Equipment name is required.');
      return;
    }
    if (!draft.equipmentId && (!draft.stationId || !draft.voltageLevelId || !draft.equipmentTypeId)) {
      this.formError.set('Station, voltage level, and equipment type are all required.');
      return;
    }

    if (draft.isOffPoint && !draft.offPointRemark.trim()) {
      this.formError.set('A remark is required when marking equipment as an Off-Point.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);

    const request$ = draft.equipmentId
      ? this.equipmentService.update(draft.equipmentId, {
          name: draft.name.trim(),
          mvaRatingId: draft.mvaRatingId,
          isOpen: draft.isOpen,
          isOffPoint: draft.isOffPoint,
          offPointRemark: draft.isOffPoint ? draft.offPointRemark.trim() : null,
          isActive: draft.isActive
        })
      : this.equipmentService.create({
          stationId: draft.stationId!,
          voltageLevelId: draft.voltageLevelId!,
          equipmentTypeId: draft.equipmentTypeId!,
          name: draft.name.trim(),
          mvaRatingId: draft.mvaRatingId,
          isOpen: draft.isOpen,
          isOffPoint: draft.isOffPoint,
          offPointRemark: draft.isOffPoint ? draft.offPointRemark.trim() : null
        });

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.cancelForm();
        this.loadEquipment();
      },
      error: (err) => {
        this.saving.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to save equipment.');
      }
    });
  }

  deactivate(equipment: Equipment): void {
    if (!confirm(`Deactivate "${equipment.equipmentName}"?`)) return;
    this.rowBusyId.set(equipment.equipmentId);
    this.equipmentService.deactivate(equipment.equipmentId).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.loadEquipment();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate equipment.');
      }
    });
  }
}
