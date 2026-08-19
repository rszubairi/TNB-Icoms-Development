import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TransmissionLineService } from '../../../core/services/transmission-line.service';
import { StationService } from '../../../core/services/station.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { ZoneService } from '../../../core/services/zone.service';
import { GeneratedName, TransmissionLine } from '../../../core/models/transmission-line.model';
import { Station } from '../../../core/models/station.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';
import { EquipmentType } from '../../../core/models/equipment-type.model';
import { Zone } from '../../../core/models/zone.model';

@Component({
  selector: 'app-transmission-lines',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './transmission-lines.component.html',
  styleUrl: './transmission-lines.component.css'
})
export class TransmissionLinesComponent {
  private lineService = inject(TransmissionLineService);
  private stationService = inject(StationService);
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentTypeService = inject(EquipmentTypeService);
  private zoneService = inject(ZoneService);

  lines = signal<TransmissionLine[]>([]);
  allStations = signal<Station[]>([]);
  voltageLevels = signal<VoltageLevel[]>([]);
  equipmentTypesForVoltage = signal<EquipmentType[]>([]);
  zones = signal<Zone[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  stationCount = signal<2 | 3 | 4>(2);
  stationIds = signal<(number | null)[]>([null, null]);
  voltageLevelId = signal<number | null>(null);
  equipmentTypeId = signal<number | null>(null);
  namingInteger = signal<number | null>(null);
  lineNumber = signal<number | null>(null);

  preview = signal<GeneratedName[]>([]);
  previewing = signal(false);
  formError = signal<string | null>(null);
  saving = signal(false);

  addZoneTarget = signal<Record<number, number | null>>({});
  zoneBusyLineId = signal<number | null>(null);
  lineBusyId = signal<number | null>(null);

  lineTypeLabel = computed(() => (this.stationCount() === 2 ? 'Single Line' : this.stationCount() === 3 ? 'Tee-Off' : 'Quad'));

  constructor() {
    this.stationService.list().subscribe({ next: (stations) => this.allStations.set(stations), error: () => {} });
    this.voltageLevelService.list().subscribe({ next: (levels) => this.voltageLevels.set(levels), error: () => {} });
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.lineService.list().subscribe({
      next: (lines) => {
        this.lines.set(lines);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load transmission lines. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  stationName(id: number | null): string {
    if (!id) return '';
    return this.allStations().find((s) => s.stationId === id)?.stationName ?? '';
  }

  // --- Wizard ---

  startCreate(): void {
    this.formError.set(null);
    this.stationCount.set(2);
    this.stationIds.set([null, null]);
    this.voltageLevelId.set(null);
    this.equipmentTypeId.set(null);
    this.namingInteger.set(null);
    this.lineNumber.set(null);
    this.preview.set([]);
    this.equipmentTypesForVoltage.set([]);
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.formError.set(null);
  }

  setStationCount(count: 2 | 3 | 4): void {
    this.stationCount.set(count);
    const current = this.stationIds();
    const next = Array.from({ length: count }, (_, i) => current[i] ?? null);
    this.stationIds.set(next);
    this.preview.set([]);
  }

  updateStationAt(index: number, value: string): void {
    const ids = [...this.stationIds()];
    ids[index] = value ? Number(value) : null;
    this.stationIds.set(ids);
    this.preview.set([]);
  }

  onVoltageChange(value: string): void {
    const voltageLevelId = value ? Number(value) : null;
    this.voltageLevelId.set(voltageLevelId);
    this.equipmentTypeId.set(null);
    this.equipmentTypesForVoltage.set([]);
    this.preview.set([]);
    if (voltageLevelId) {
      this.equipmentTypeService.list(voltageLevelId).subscribe({
        next: (types) => this.equipmentTypesForVoltage.set(types.filter((t) => t.isActive)),
        error: () => {}
      });
    }
  }

  onEquipmentTypeChange(value: string): void {
    this.equipmentTypeId.set(value ? Number(value) : null);
    this.preview.set([]);
  }

  updateNamingInteger(value: number): void {
    this.namingInteger.set(value);
    this.preview.set([]);
  }

  updateLineNumber(value: number): void {
    this.lineNumber.set(value);
    this.preview.set([]);
  }

  private buildRequest() {
    const ids = this.stationIds();
    if (ids.some((id) => !id) || !this.voltageLevelId() || !this.equipmentTypeId() || !this.namingInteger() || !this.lineNumber()) {
      return null;
    }
    return {
      voltageLevelId: this.voltageLevelId()!,
      equipmentTypeId: this.equipmentTypeId()!,
      namingInteger: this.namingInteger()!,
      lineNumber: this.lineNumber()!,
      stationIdsInOrder: ids as number[]
    };
  }

  generatePreview(): void {
    const request = this.buildRequest();
    if (!request) {
      this.formError.set('Fill in every station, voltage, equipment type, naming integer, and line number first.');
      return;
    }

    this.formError.set(null);
    this.previewing.set(true);
    this.lineService.preview(request).subscribe({
      next: (names) => {
        this.previewing.set(false);
        this.preview.set(names);
      },
      error: (err) => {
        this.previewing.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to generate preview.');
      }
    });
  }

  save(): void {
    const request = this.buildRequest();
    if (!request) {
      this.formError.set('Fill in every station, voltage, equipment type, naming integer, and line number first.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);
    this.lineService.create(request).subscribe({
      next: () => {
        this.saving.set(false);
        this.showForm.set(false);
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to save line.');
      }
    });
  }

  // --- Owner zones ---

  onAddZoneTargetChange(lineId: number, value: string): void {
    this.addZoneTarget.update((m) => ({ ...m, [lineId]: value ? Number(value) : null }));
  }

  addOwnerZone(line: TransmissionLine): void {
    const zoneId = this.addZoneTarget()[line.transmissionLineId];
    if (!zoneId) return;

    this.zoneBusyLineId.set(line.transmissionLineId);
    this.lineService.addOwnerZone(line.transmissionLineId, zoneId).subscribe({
      next: () => {
        this.zoneBusyLineId.set(null);
        this.addZoneTarget.update((m) => ({ ...m, [line.transmissionLineId]: null }));
        this.load();
      },
      error: (err) => {
        this.zoneBusyLineId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to add owner zone.');
      }
    });
  }

  removeOwnerZone(line: TransmissionLine, zoneId: number): void {
    this.zoneBusyLineId.set(line.transmissionLineId);
    this.lineService.removeOwnerZone(line.transmissionLineId, zoneId).subscribe({
      next: () => {
        this.zoneBusyLineId.set(null);
        this.load();
      },
      error: (err) => {
        this.zoneBusyLineId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to remove owner zone.');
      }
    });
  }

  deactivateLine(line: TransmissionLine): void {
    if (!confirm(`Deactivate this line and all its generated equipment records?`)) return;
    this.lineBusyId.set(line.transmissionLineId);
    this.lineService.deactivate(line.transmissionLineId).subscribe({
      next: () => {
        this.lineBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.lineBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate line.');
      }
    });
  }
}
