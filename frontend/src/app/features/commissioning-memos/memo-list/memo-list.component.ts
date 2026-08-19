import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommissioningMemoService } from '../../../core/services/commissioning-memo.service';
import { OutageService } from '../../../core/services/outage.service';
import { CommissioningMemoListItem } from '../../../core/models/commissioning-memo.model';

const STATUS_LABELS: Record<string, string> = {
  PendingEngineerPic: 'Pending Engineer PIC',
  PendingSE: 'Pending S/E',
  PendingDCE: 'Pending DCE',
  PendingCeGnm: 'Pending CE GNM',
  PendingFinalSignOff: 'Pending Final Sign-off',
  Approved: 'Approved'
};

@Component({
  selector: 'app-memo-list',
  standalone: true,
  imports: [DatePipe, FormsModule, RouterLink],
  templateUrl: './memo-list.component.html',
  styleUrl: './memo-list.component.css'
})
export class MemoListComponent {
  private memoService = inject(CommissioningMemoService);
  private outageService = inject(OutageService);

  memos = signal<CommissioningMemoListItem[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  formOutageId = signal<number | null>(null);
  outageLookupResult = signal<string | null>(null);
  outageLookupError = signal<string | null>(null);
  formMemoType = signal('Commissioning');
  formSwitchingProgram = signal('');
  formDataForm = signal('');
  formIomEndorsed = signal(false);
  formMtepProtectionLetter = signal(false);
  formResidentEngineerCertification = signal(false);
  formFormG = signal(false);
  formFormH = signal(false);
  formMeteringEmailChain = signal(false);
  formScadaEmailChain = signal(false);
  formHgsoLetterForGenerationPmu = signal(false);
  formError = signal<string | null>(null);
  saving = signal(false);

  statusLabel(status: string): string {
    return STATUS_LABELS[status] ?? status;
  }

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.memoService.list().subscribe({
      next: (memos) => { this.memos.set(memos); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load Commissioning Memos. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  openForm(): void {
    this.formOutageId.set(null);
    this.outageLookupResult.set(null);
    this.outageLookupError.set(null);
    this.formMemoType.set('Commissioning');
    this.formSwitchingProgram.set('');
    this.formDataForm.set('');
    this.formIomEndorsed.set(false);
    this.formMtepProtectionLetter.set(false);
    this.formResidentEngineerCertification.set(false);
    this.formFormG.set(false);
    this.formFormH.set(false);
    this.formMeteringEmailChain.set(false);
    this.formScadaEmailChain.set(false);
    this.formHgsoLetterForGenerationPmu.set(false);
    this.formError.set(null);
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
  }

  lookupOutage(): void {
    const id = this.formOutageId();
    if (!id) return;
    this.outageLookupResult.set(null);
    this.outageLookupError.set(null);
    this.outageService.getById(id).subscribe({
      next: (o) => this.outageLookupResult.set(`${o.outageNumber} — ${o.stationName} · ${o.description}`),
      error: () => this.outageLookupError.set('Outage not found.')
    });
  }

  submit(): void {
    if (!this.formOutageId() || !this.formSwitchingProgram().trim()) {
      this.formError.set('Outage and Switching Program are required.');
      return;
    }
    const isEmergency = this.formMemoType() === 'EmergencyCommissioning';
    if (!isEmergency && !this.formDataForm().trim()) {
      this.formError.set('Data Form is required for this memo type.');
      return;
    }

    this.saving.set(true);
    this.formError.set(null);
    this.memoService.create({
      outageId: this.formOutageId()!,
      memoType: this.formMemoType(),
      switchingProgram: this.formSwitchingProgram().trim(),
      dataForm: isEmergency ? null : this.formDataForm().trim(),
      iomEndorsed: this.formIomEndorsed(),
      mtepProtectionLetter: this.formMtepProtectionLetter(),
      residentEngineerCertification: this.formResidentEngineerCertification(),
      formG: this.formFormG(),
      formH: this.formFormH(),
      meteringEmailChain: this.formMeteringEmailChain(),
      scadaEmailChain: this.formScadaEmailChain(),
      hgsoLetterForGenerationPmu: this.formHgsoLetterForGenerationPmu()
    }).subscribe({
      next: () => { this.saving.set(false); this.showForm.set(false); this.load(); },
      error: (err) => { this.saving.set(false); this.formError.set(err?.error?.error ?? 'Unable to submit.'); }
    });
  }
}
