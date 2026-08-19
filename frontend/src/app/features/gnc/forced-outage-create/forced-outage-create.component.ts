import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { GncService } from '../../../core/services/gnc.service';
import { ZoneService } from '../../../core/services/zone.service';
import { StationService } from '../../../core/services/station.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { EquipmentService } from '../../../core/services/equipment.service';
import { DropdownValueService } from '../../../core/services/dropdown-value.service';
import { AuthorisationPersonnelService } from '../../../core/services/authorisation-personnel.service';
import { AuthService } from '../../../core/services/auth.service';
import { Zone } from '../../../core/models/zone.model';
import { Station } from '../../../core/models/station.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';
import { EquipmentType } from '../../../core/models/equipment-type.model';
import { Equipment } from '../../../core/models/equipment.model';
import { DropdownValue } from '../../../core/models/dropdown-value.model';
import { AuthorisationPersonnel } from '../../../core/models/authorisation-personnel.model';
import { OutagePic } from '../../../core/models/outage.model';

interface PicDraft {
  picName: string;
  picEmail: string;
  picPhone: string;
}

@Component({
  selector: 'app-forced-outage-create',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './forced-outage-create.component.html',
  styleUrl: './forced-outage-create.component.css'
})
export class ForcedOutageCreateComponent {
  private gncService = inject(GncService);
  private zoneService = inject(ZoneService);
  private stationService = inject(StationService);
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentTypeService = inject(EquipmentTypeService);
  private equipmentService = inject(EquipmentService);
  private dropdownValueService = inject(DropdownValueService);
  private personnelService = inject(AuthorisationPersonnelService);
  private authService = inject(AuthService);
  private router = inject(Router);

  zones = signal<Zone[]>([]);
  allStations = signal<Station[]>([]);
  voltageLevels = signal<VoltageLevel[]>([]);
  equipmentTypes = signal<EquipmentType[]>([]);
  equipmentList = signal<Equipment[]>([]);
  additionalEquipmentTypes = signal<EquipmentType[]>([]);
  additionalEquipmentList = signal<Equipment[]>([]);
  workTypes = signal<DropdownValue[]>([]);
  jobTypes = signal<DropdownValue[]>([]);
  approvers = signal<AuthorisationPersonnel[]>([]);

  zoneId = signal<number | null>(null);
  stationId = signal<number | null>(null);
  voltageLevelId = signal<number | null>(null);
  equipmentTypeId = signal<number | null>(null);
  primaryEquipmentId = signal<number | null>(null);
  additionalEquipmentTypeId = signal<number | null>(null);
  additionalEquipmentIds = signal<number[]>([]);
  workTypeCode = signal<string>('Dead');
  hasPtw = signal(false);
  startDate = signal('');
  startTime = signal('08:00');
  endDate = signal('');
  endTime = signal('17:00');
  jobTypeId = signal<number | null>(null);
  approvedById = signal<number | null>(null);
  description = signal('');
  pics = signal<PicDraft[]>([]);

  showPicForm = signal(false);
  picDraft = signal<PicDraft>({ picName: '', picEmail: '', picPhone: '' });

  saving = signal(false);
  formError = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  stationsForZone = computed(() => {
    const zoneId = this.zoneId();
    return zoneId ? this.allStations().filter((s) => s.zoneId === zoneId) : [];
  });

  constructor() {
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
    this.stationService.list().subscribe({ next: (stations) => this.allStations.set(stations), error: () => {} });
    this.voltageLevelService.list().subscribe({ next: (levels) => this.voltageLevels.set(levels), error: () => {} });
    this.dropdownValueService.listByCategory('WorkType').subscribe({ next: (v) => this.workTypes.set(v), error: () => {} });
    this.dropdownValueService.listByCategory('JobType').subscribe({ next: (v) => this.jobTypes.set(v), error: () => {} });
    this.personnelService.list().subscribe({ next: (p) => this.approvers.set(p.filter((x) => x.isActive)), error: () => {} });

    const zone = this.authService.currentUser()?.zoneId;
    if (zone) this.zoneId.set(Number(zone));

    const today = new Date().toISOString().slice(0, 10);
    this.startDate.set(today);
    this.endDate.set(today);
  }

  onZoneChange(value: string): void {
    this.zoneId.set(value ? Number(value) : null);
    this.stationId.set(null);
  }

  onStationChange(value: string): void {
    this.stationId.set(value ? Number(value) : null);
  }

  onVoltageChange(value: string): void {
    const voltageLevelId = value ? Number(value) : null;
    this.voltageLevelId.set(voltageLevelId);
    this.equipmentTypeId.set(null);
    this.primaryEquipmentId.set(null);
    this.equipmentTypes.set([]);
    this.equipmentList.set([]);
    this.additionalEquipmentTypes.set([]);
    if (voltageLevelId) {
      this.equipmentTypeService.list(voltageLevelId).subscribe({
        next: (types) => {
          this.equipmentTypes.set(types.filter((t) => t.isActive));
          this.additionalEquipmentTypes.set(types.filter((t) => t.isActive));
        },
        error: () => {}
      });
    }
  }

  onEquipmentTypeChange(value: string): void {
    const equipmentTypeId = value ? Number(value) : null;
    this.equipmentTypeId.set(equipmentTypeId);
    this.primaryEquipmentId.set(null);
    this.equipmentList.set([]);
    if (equipmentTypeId && this.stationId()) {
      this.equipmentService.list({ stationId: this.stationId()!, equipmentTypeId }).subscribe({
        next: (equipment) => this.equipmentList.set(equipment.filter((e) => e.isActive)),
        error: () => {}
      });
    }
  }

  onPrimaryEquipmentChange(value: string): void {
    this.primaryEquipmentId.set(value ? Number(value) : null);
  }

  onAdditionalTypeChange(value: string): void {
    const typeId = value ? Number(value) : null;
    this.additionalEquipmentTypeId.set(typeId);
    this.additionalEquipmentList.set([]);
    if (typeId && this.stationId()) {
      this.equipmentService.list({ stationId: this.stationId()!, equipmentTypeId: typeId }).subscribe({
        next: (equipment) => this.additionalEquipmentList.set(equipment.filter((e) => e.isActive)),
        error: () => {}
      });
    }
  }

  toggleAdditionalEquipment(id: number, checked: boolean): void {
    const current = this.additionalEquipmentIds();
    this.additionalEquipmentIds.set(checked ? [...current, id] : current.filter((x) => x !== id));
  }

  isAdditionalChecked(id: number): boolean {
    return this.additionalEquipmentIds().includes(id);
  }

  openPicForm(): void {
    this.picDraft.set({ picName: '', picEmail: '', picPhone: '' });
    this.showPicForm.set(true);
  }

  cancelPicForm(): void {
    this.showPicForm.set(false);
  }

  updatePicField<K extends keyof PicDraft>(field: K, value: PicDraft[K]): void {
    this.picDraft.update((d) => ({ ...d, [field]: value }));
  }

  addPic(): void {
    const draft = this.picDraft();
    if (!draft.picName.trim() || !draft.picEmail.trim()) return;
    this.pics.update((list) => [...list, { ...draft }]);
    this.showPicForm.set(false);
  }

  removePic(index: number): void {
    this.pics.update((list) => list.filter((_, i) => i !== index));
  }

  submit(): void {
    if (
      !this.stationId() || !this.voltageLevelId() || !this.equipmentTypeId() || !this.primaryEquipmentId() ||
      !this.jobTypeId() || !this.approvedById() || !this.description().trim() ||
      !this.startDate() || !this.endDate()
    ) {
      this.formError.set('Station, voltage, equipment, job type, approver, and description are all required.');
      return;
    }

    const picsPayload: OutagePic[] = this.pics().map((p) => ({
      picName: p.picName,
      picEmail: p.picEmail,
      picPhone: p.picPhone || null
    }));

    this.formError.set(null);
    this.saving.set(true);

    this.gncService.createForcedOutage({
      stationId: this.stationId()!,
      voltageLevelId: this.voltageLevelId()!,
      equipmentTypeId: this.equipmentTypeId()!,
      primaryEquipmentId: this.primaryEquipmentId()!,
      lineFilterType: null,
      additionalEquipmentIds: this.additionalEquipmentIds(),
      workTypeCode: this.workTypeCode(),
      hasPtw: this.hasPtw(),
      plannedStartAt: `${this.startDate()}T${this.startTime()}:00`,
      plannedEndAt: `${this.endDate()}T${this.endTime()}:00`,
      jobTypeId: this.jobTypeId()!,
      description: this.description().trim(),
      approvedById: this.approvedById()!,
      pics: picsPayload
    }).subscribe({
      next: (outage) => {
        this.saving.set(false);
        this.successMessage.set(`Forced outage ${outage.outageNumber} created and approved.`);
      },
      error: (err) => {
        this.saving.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to create forced outage.');
      }
    });
  }

  goToScheduled(): void {
    this.router.navigateByUrl('/gnc/scheduled');
  }
}
