import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LinkingLineService } from '../../../core/services/linking-line.service';
import { ZoneService } from '../../../core/services/zone.service';
import { StationService } from '../../../core/services/station.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { EquipmentService } from '../../../core/services/equipment.service';
import { LinkingLine } from '../../../core/models/linking-line.model';
import { Zone } from '../../../core/models/zone.model';
import { Station } from '../../../core/models/station.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';
import { EquipmentType } from '../../../core/models/equipment-type.model';
import { Equipment } from '../../../core/models/equipment.model';

interface LinePicker {
  zoneId: number | null;
  stationId: number | null;
  voltageLevelId: number | null;
  equipmentTypeId: number | null;
  equipmentId: number | null;
  stations: Station[];
  equipmentTypes: EquipmentType[];
  equipment: Equipment[];
}

function emptyPicker(): LinePicker {
  return { zoneId: null, stationId: null, voltageLevelId: null, equipmentTypeId: null, equipmentId: null, stations: [], equipmentTypes: [], equipment: [] };
}

@Component({
  selector: 'app-linking-lines',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './linking-lines.component.html',
  styleUrl: './linking-lines.component.css'
})
export class LinkingLinesComponent {
  private linkingLineService = inject(LinkingLineService);
  private zoneService = inject(ZoneService);
  private stationService = inject(StationService);
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentTypeService = inject(EquipmentTypeService);
  private equipmentService = inject(EquipmentService);

  pairs = signal<LinkingLine[]>([]);
  zones = signal<Zone[]>([]);
  allStations = signal<Station[]>([]);
  voltageLevels = signal<VoltageLevel[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  line1 = signal<LinePicker>(emptyPicker());
  line2 = signal<LinePicker>(emptyPicker());
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
    this.linkingLineService.list().subscribe({
      next: (pairs) => {
        this.pairs.set(pairs);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load linking lines. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  startCreate(): void {
    this.formError.set(null);
    this.line1.set(emptyPicker());
    this.line2.set(emptyPicker());
    this.remark.set('');
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.formError.set(null);
  }

  onZoneChange(side: 1 | 2, value: string): void {
    const target = side === 1 ? this.line1 : this.line2;
    const zoneId = value ? Number(value) : null;
    target.set({ ...emptyPicker(), zoneId, stations: zoneId ? this.allStations().filter((s) => s.zoneId === zoneId) : [] });
  }

  onStationChange(side: 1 | 2, value: string): void {
    const target = side === 1 ? this.line1 : this.line2;
    const stationId = value ? Number(value) : null;
    target.update((e) => ({ ...e, stationId, voltageLevelId: null, equipmentTypeId: null, equipmentId: null, equipmentTypes: [], equipment: [] }));
  }

  onVoltageChange(side: 1 | 2, value: string): void {
    const target = side === 1 ? this.line1 : this.line2;
    const voltageLevelId = value ? Number(value) : null;
    target.update((e) => ({ ...e, voltageLevelId, equipmentTypeId: null, equipmentId: null, equipment: [] }));
    if (voltageLevelId) {
      this.equipmentTypeService.list(voltageLevelId).subscribe({
        next: (types) => target.update((e) => ({ ...e, equipmentTypes: types.filter((t) => t.isActive) })),
        error: () => {}
      });
    }
  }

  onEquipmentTypeChange(side: 1 | 2, value: string): void {
    const target = side === 1 ? this.line1 : this.line2;
    const equipmentTypeId = value ? Number(value) : null;
    const picker = target();
    target.update((e) => ({ ...e, equipmentTypeId, equipmentId: null, equipment: [] }));
    if (equipmentTypeId && picker.stationId) {
      this.equipmentService.list({ stationId: picker.stationId, equipmentTypeId }).subscribe({
        next: (equipment) => target.update((e) => ({ ...e, equipment: equipment.filter((eq) => eq.isActive) })),
        error: () => {}
      });
    }
  }

  onEquipmentChange(side: 1 | 2, value: string): void {
    const target = side === 1 ? this.line1 : this.line2;
    target.update((e) => ({ ...e, equipmentId: value ? Number(value) : null }));
  }

  save(): void {
    const a = this.line1();
    const b = this.line2();

    if (!a.equipmentId || !b.equipmentId) {
      this.formError.set('Select equipment for both Line 1 and Line 2.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);

    this.linkingLineService
      .create({ equipmentId: a.equipmentId, linkedEquipmentId: b.equipmentId, remark: this.remark().trim() || null })
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

  deactivate(pair: LinkingLine): void {
    if (!confirm('Remove this linking line pair?')) return;
    this.rowBusyId.set(pair.linkingLineId);
    this.linkingLineService.deactivate(pair.linkingLineId).subscribe({
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
