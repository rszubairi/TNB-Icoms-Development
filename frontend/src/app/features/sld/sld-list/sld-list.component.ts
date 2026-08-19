import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SldService } from '../../../core/services/sld.service';
import { ZoneService } from '../../../core/services/zone.service';
import { StationService } from '../../../core/services/station.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { AuthService } from '../../../core/services/auth.service';
import { SldListItem } from '../../../core/models/sld.model';
import { Zone } from '../../../core/models/zone.model';
import { Station } from '../../../core/models/station.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';

const STATUS_LABELS: Record<string, string> = {
  PendingEngineerReview: 'Pending Engineer Review',
  PendingSE: 'Pending S/E',
  PendingDCE: 'Pending DCE',
  PendingRequestorApproval: 'Pending Requestor Approval',
  Published: 'Published',
  Rejected: 'Rejected'
};

@Component({
  selector: 'app-sld-list',
  standalone: true,
  imports: [DatePipe, FormsModule, RouterLink],
  templateUrl: './sld-list.component.html',
  styleUrl: './sld-list.component.css'
})
export class SldListComponent {
  private sldService = inject(SldService);
  private zoneService = inject(ZoneService);
  private stationService = inject(StationService);
  private voltageLevelService = inject(VoltageLevelService);
  private authService = inject(AuthService);

  diagrams = signal<SldListItem[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  zones = signal<Zone[]>([]);
  allStations = signal<Station[]>([]);
  voltageLevels = signal<VoltageLevel[]>([]);

  showForm = signal(false);
  formZoneId = signal<number | null>(null);
  formStationId = signal<number | null>(null);
  formVoltageLevelId = signal<number | null>(null);
  formFlowType = signal('New');
  formRemark = signal('');
  formError = signal<string | null>(null);
  saving = signal(false);

  stationsForZone = computed(() => {
    const zoneId = this.formZoneId();
    return zoneId ? this.allStations().filter((s) => s.zoneId === zoneId) : [];
  });

  statusLabel(status: string): string {
    return STATUS_LABELS[status] ?? status;
  }

  constructor() {
    this.zoneService.list().subscribe({ next: (z) => this.zones.set(z), error: () => {} });
    this.stationService.list().subscribe({ next: (s) => this.allStations.set(s), error: () => {} });
    this.voltageLevelService.list().subscribe({ next: (v) => this.voltageLevels.set(v), error: () => {} });
    const zone = this.authService.currentUser()?.zoneId;
    if (zone) this.formZoneId.set(Number(zone));
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.sldService.list().subscribe({
      next: (diagrams) => { this.diagrams.set(diagrams); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load Single Line Diagrams. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  onFormZoneChange(value: string): void {
    this.formZoneId.set(value ? Number(value) : null);
    this.formStationId.set(null);
  }

  openForm(): void {
    this.formStationId.set(null);
    this.formVoltageLevelId.set(null);
    this.formFlowType.set('New');
    this.formRemark.set('');
    this.formError.set(null);
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
  }

  submit(): void {
    if (!this.formStationId() || !this.formVoltageLevelId()) {
      this.formError.set('Station and voltage are required.');
      return;
    }
    this.saving.set(true);
    this.formError.set(null);
    this.sldService.create({
      stationId: this.formStationId()!,
      voltageLevelId: this.formVoltageLevelId()!,
      flowType: this.formFlowType(),
      remark: this.formRemark() || null
    }).subscribe({
      next: () => { this.saving.set(false); this.showForm.set(false); this.load(); },
      error: (err) => { this.saving.set(false); this.formError.set(err?.error?.error ?? 'Unable to submit.'); }
    });
  }
}
