import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { OutageService } from '../../../core/services/outage.service';
import { ChangeRequestService } from '../../../core/services/change-request.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { EquipmentService } from '../../../core/services/equipment.service';
import { OutageDetail } from '../../../core/models/outage.model';
import { ChangeRequestBatch } from '../../../core/models/change-request.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';
import { Equipment } from '../../../core/models/equipment.model';

@Component({
  selector: 'app-outage-detail',
  standalone: true,
  imports: [RouterLink, FormsModule, DatePipe],
  templateUrl: './outage-detail.component.html',
  styleUrl: './outage-detail.component.css'
})
export class OutageDetailComponent {
  private route = inject(ActivatedRoute);
  private outageService = inject(OutageService);
  private changeRequestService = inject(ChangeRequestService);
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentService = inject(EquipmentService);

  outageId = Number(this.route.snapshot.paramMap.get('id'));

  outage = signal<OutageDetail | null>(null);
  batches = signal<ChangeRequestBatch[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  voltageLevels = signal<VoltageLevel[]>([]);
  equipmentOptions = signal<Equipment[]>([]);

  canRequestChange = computed(() => {
    const o = this.outage();
    if (!o) return false;
    return o.gnmStatus !== 'Approved' && !o.gnmStatus.startsWith('Outage Closed') && !o.requestorStatus.startsWith('Outage Closed');
  });

  hasPendingBatch = computed(() => this.batches().some((b) => b.status === 'Pending'));

  showForm = signal(false);
  crReason = signal('');
  crNewStartDate = signal('');
  crNewStartTime = signal('');
  crNewEndDate = signal('');
  crNewEndTime = signal('');
  crNewVoltageLevelId = signal<number | null>(null);
  crNewEquipmentId = signal<number | null>(null);
  formError = signal<string | null>(null);
  saving = signal(false);

  // --- GNM Study & Approval (URS Module 3 §5.3-5.4) ---
  studyJustification = signal('');
  studyHighlights = signal('');
  studyRemark = signal('');
  studyUnderStudyNotes = signal('');
  studySaving = signal(false);
  studyActionError = signal<string | null>(null);
  studyActionBusy = signal(false);

  canGnmApprove = computed(() => {
    const o = this.outage();
    if (!o) return false;
    return o.requestorStatus === 'Confirmed' && o.plannerStatus === 'Agreed'
      && (o.gnmStatus === 'Pending' || o.gnmStatus === 'Under-Study' || o.gnmStatus === 'KIV');
  });

  canGnmRevert = computed(() => {
    const o = this.outage();
    if (!o) return false;
    return o.gnmStatus === 'Approved' && (!o.gncStatus || o.gncStatus === 'Outage Closed - Not Taken');
  });

  constructor() {
    this.voltageLevelService.list().subscribe({ next: (v) => this.voltageLevels.set(v), error: () => {} });
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.outageService.getById(this.outageId).subscribe({
      next: (outage) => {
        this.outage.set(outage);
        this.loading.set(false);
        this.studyJustification.set(outage.justification ?? '');
        this.studyHighlights.set(outage.highlights ?? '');
        this.studyRemark.set(outage.remark ?? '');
        this.studyUnderStudyNotes.set(outage.underStudyNotes ?? '');
        if (outage.voltageLevelId) {
          this.equipmentService.list({ voltageLevelId: outage.voltageLevelId }).subscribe({
            next: (eq) => this.equipmentOptions.set(eq.filter((e) => e.isActive)),
            error: () => {}
          });
        }
      },
      error: () => {
        this.errorMessage.set('Unable to load this outage. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });

    this.changeRequestService.listForOutage(this.outageId).subscribe({ next: (b) => this.batches.set(b), error: () => {} });
  }

  openForm(): void {
    this.crReason.set('');
    this.crNewStartDate.set('');
    this.crNewStartTime.set('');
    this.crNewEndDate.set('');
    this.crNewEndTime.set('');
    this.crNewVoltageLevelId.set(null);
    this.crNewEquipmentId.set(null);
    this.formError.set(null);
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
  }

  submitChangeRequest(): void {
    if (!this.crReason().trim()) {
      this.formError.set('A reason is required.');
      return;
    }

    const hasScheduleChange = !!(this.crNewStartDate() && this.crNewStartTime() && this.crNewEndDate() && this.crNewEndTime());
    const hasVoltageChange = !!this.crNewVoltageLevelId();
    const hasEquipmentChange = !!this.crNewEquipmentId();

    if (!hasScheduleChange && !hasVoltageChange && !hasEquipmentChange) {
      this.formError.set('Change at least one of: date/time, voltage, or equipment.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);

    this.changeRequestService
      .create({
        outageId: this.outageId,
        reason: this.crReason().trim(),
        newPlannedStartAt: hasScheduleChange ? `${this.crNewStartDate()}T${this.crNewStartTime()}:00` : null,
        newPlannedEndAt: hasScheduleChange ? `${this.crNewEndDate()}T${this.crNewEndTime()}:00` : null,
        newVoltageLevelId: this.crNewVoltageLevelId(),
        newPrimaryEquipmentId: this.crNewEquipmentId(),
        addAdditionalEquipmentIds: []
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.showForm.set(false);
          this.load();
        },
        error: (err) => {
          this.saving.set(false);
          this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to submit change request.');
        }
      });
  }

  startStudy(): void {
    this.studyActionBusy.set(true);
    this.studyActionError.set(null);
    this.outageService.startStudy(this.outageId).subscribe({
      next: () => { this.studyActionBusy.set(false); this.load(); },
      error: (err) => { this.studyActionBusy.set(false); this.studyActionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  saveStudy(notify: boolean): void {
    this.studySaving.set(true);
    this.studyActionError.set(null);
    this.outageService.updateStudy(this.outageId, {
      justification: this.studyJustification() || null,
      highlights: this.studyHighlights() || null,
      remark: this.studyRemark() || null,
      underStudyNotes: this.studyUnderStudyNotes() || null
    }, notify).subscribe({
      next: () => { this.studySaving.set(false); this.load(); },
      error: (err) => { this.studySaving.set(false); this.studyActionError.set(err?.error?.error ?? 'Unable to save study notes.'); }
    });
  }

  setKiv(): void {
    this.studyActionBusy.set(true);
    this.studyActionError.set(null);
    this.outageService.setKiv(this.outageId).subscribe({
      next: () => { this.studyActionBusy.set(false); this.load(); },
      error: (err) => { this.studyActionBusy.set(false); this.studyActionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  gnmApprove(): void {
    this.studyActionBusy.set(true);
    this.studyActionError.set(null);
    this.outageService.gnmApprove(this.outageId).subscribe({
      next: () => { this.studyActionBusy.set(false); this.load(); },
      error: (err) => { this.studyActionBusy.set(false); this.studyActionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  gnmDisapprove(): void {
    if (!confirm('Disapprove this outage?')) return;
    this.studyActionBusy.set(true);
    this.studyActionError.set(null);
    this.outageService.gnmDisapprove(this.outageId).subscribe({
      next: () => { this.studyActionBusy.set(false); this.load(); },
      error: (err) => { this.studyActionBusy.set(false); this.studyActionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  gnmRevert(): void {
    if (!confirm('Revert this outage back to Under-Study?')) return;
    this.studyActionBusy.set(true);
    this.studyActionError.set(null);
    this.outageService.gnmRevert(this.outageId).subscribe({
      next: () => { this.studyActionBusy.set(false); this.load(); },
      error: (err) => { this.studyActionBusy.set(false); this.studyActionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }
}
