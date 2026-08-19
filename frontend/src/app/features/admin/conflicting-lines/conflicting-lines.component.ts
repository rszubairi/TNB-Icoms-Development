import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConflictingLineService } from '../../../core/services/conflicting-line.service';
import { ZoneService } from '../../../core/services/zone.service';
import { StationService } from '../../../core/services/station.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { EquipmentService } from '../../../core/services/equipment.service';
import { ConflictingLine } from '../../../core/models/conflicting-line.model';
import { Zone } from '../../../core/models/zone.model';
import { Station } from '../../../core/models/station.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';
import { EquipmentType } from '../../../core/models/equipment-type.model';
import { Equipment } from '../../../core/models/equipment.model';

interface EndPicker {
  zoneId: number | null;
  stationId: number | null;
  voltageLevelId: number | null;
  equipmentTypeId: number | null;
  equipmentId: number | null;
  stations: Station[];
  equipmentTypes: EquipmentType[];
  equipment: Equipment[];
}

function emptyEnd(): EndPicker {
  return { zoneId: null, stationId: null, voltageLevelId: null, equipmentTypeId: null, equipmentId: null, stations: [], equipmentTypes: [], equipment: [] };
}

@Component({
  selector: 'app-conflicting-lines',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './conflicting-lines.component.html',
  styleUrl: './conflicting-lines.component.css'
})
export class ConflictingLinesComponent {
  private conflictingLineService = inject(ConflictingLineService);
  private zoneService = inject(ZoneService);
  private stationService = inject(StationService);
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentTypeService = inject(EquipmentTypeService);
  private equipmentService = inject(EquipmentService);

  pairs = signal<ConflictingLine[]>([]);
  zones = signal<Zone[]>([]);
  allStations = signal<Station[]>([]);
  voltageLevels = signal<VoltageLevel[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  nearEnd = signal<EndPicker>(emptyEnd());
  farEnd = signal<EndPicker>(emptyEnd());
  remark = signal('');
  formError = signal<string | null>(null);
  saving = signal(false);
  rowBusyId = signal<number | null>(null);

  constructor() {
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
    this.stationService.list().subscribe({ next: (stations) => this.allStations.set(stations), error: () => {} });
    this.voltageLevelService.list().subscribe({ next: (levels) => this.voltageLevels.set(levels), error: () => {} });
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.conflictingLineService.list().subscribe({
      next: (pairs) => {
        this.pairs.set(pairs);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load conflicting lines. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  startCreate(): void {
    this.formError.set(null);
    this.nearEnd.set(emptyEnd());
    this.farEnd.set(emptyEnd());
    this.remark.set('');
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.formError.set(null);
  }

  onZoneChange(side: 'near' | 'far', value: string): void {
    const target = side === 'near' ? this.nearEnd : this.farEnd;
    const zoneId = value ? Number(value) : null;
    target.set({ ...emptyEnd(), zoneId, stations: zoneId ? this.allStations().filter((s) => s.zoneId === zoneId) : [] });
  }

  onStationChange(side: 'near' | 'far', value: string): void {
    const target = side === 'near' ? this.nearEnd : this.farEnd;
    const stationId = value ? Number(value) : null;
    target.update((e) => ({ ...e, stationId, voltageLevelId: null, equipmentTypeId: null, equipmentId: null, equipmentTypes: [], equipment: [] }));
  }

  onVoltageChange(side: 'near' | 'far', value: string): void {
    const target = side === 'near' ? this.nearEnd : this.farEnd;
    const voltageLevelId = value ? Number(value) : null;
    target.update((e) => ({ ...e, voltageLevelId, equipmentTypeId: null, equipmentId: null, equipment: [] }));
    if (voltageLevelId) {
      this.equipmentTypeService.list(voltageLevelId).subscribe({
        next: (types) => target.update((e) => ({ ...e, equipmentTypes: types.filter((t) => t.isActive) })),
        error: () => {}
      });
    }
  }

  onEquipmentTypeChange(side: 'near' | 'far', value: string): void {
    const target = side === 'near' ? this.nearEnd : this.farEnd;
    const equipmentTypeId = value ? Number(value) : null;
    const end = target();
    target.update((e) => ({ ...e, equipmentTypeId, equipmentId: null, equipment: [] }));
    if (equipmentTypeId && end.stationId) {
      this.equipmentService.list({ stationId: end.stationId, equipmentTypeId }).subscribe({
        next: (equipment) => target.update((e) => ({ ...e, equipment: equipment.filter((eq) => eq.isActive) })),
        error: () => {}
      });
    }
  }

  onEquipmentChange(side: 'near' | 'far', value: string): void {
    const target = side === 'near' ? this.nearEnd : this.farEnd;
    target.update((e) => ({ ...e, equipmentId: value ? Number(value) : null }));
  }

  save(): void {
    const near = this.nearEnd();
    const far = this.farEnd();

    if (!near.equipmentId || !far.equipmentId) {
      this.formError.set('Select equipment for both the near end and far end.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);

    this.conflictingLineService
      .create({ equipmentId: near.equipmentId, conflictingEquipmentId: far.equipmentId, remark: this.remark().trim() || null })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.showForm.set(false);
          this.load();
        },
        error: (err) => {
          this.saving.set(false);
          this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to save pair.');
        }
      });
  }

  deactivate(pair: ConflictingLine): void {
    if (!confirm('Remove this conflicting line pair?')) return;
    this.rowBusyId.set(pair.conflictingLineId);
    this.conflictingLineService.deactivate(pair.conflictingLineId).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to remove pair.');
      }
    });
  }
}
