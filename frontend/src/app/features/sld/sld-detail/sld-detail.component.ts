import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SldService } from '../../../core/services/sld.service';
import { SldDetail } from '../../../core/models/sld.model';

@Component({
  selector: 'app-sld-detail',
  standalone: true,
  imports: [DatePipe, FormsModule, RouterLink],
  templateUrl: './sld-detail.component.html',
  styleUrl: './sld-detail.component.css'
})
export class SldDetailComponent {
  private route = inject(ActivatedRoute);
  private sldService = inject(SldService);

  sldId = Number(this.route.snapshot.paramMap.get('id'));

  sld = signal<SldDetail | null>(null);
  loading = signal(true);
  errorMessage = signal<string | null>(null);
  actionError = signal<string | null>(null);
  busy = signal(false);

  uploading = signal(false);
  reviewMnemonic = signal('');
  reviewSubstationType = signal('AIS');
  rejectionReason = signal('');

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.sldService.getById(this.sldId).subscribe({
      next: (sld) => {
        this.sld.set(sld);
        this.loading.set(false);
        this.reviewMnemonic.set(sld.mnemonic ?? '');
        this.reviewSubstationType.set(sld.substationType ?? 'AIS');
      },
      error: () => { this.errorMessage.set('Unable to load this diagram. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    this.uploading.set(true);
    this.actionError.set(null);
    this.sldService.uploadDrawing(this.sldId, file).subscribe({
      next: () => { this.uploading.set(false); this.load(); },
      error: (err) => { this.uploading.set(false); this.actionError.set(err?.error?.error ?? 'Upload failed.'); }
    });
  }

  downloadDrawing(): void {
    this.sldService.downloadDrawing(this.sldId).subscribe({
      next: (blob) => this.sldService.triggerDownload(blob, `sld-${this.sld()?.diagramNumber ?? this.sldId}`),
      error: () => this.actionError.set('Unable to download drawing.')
    });
  }

  submitEngineerApprove(): void {
    if (!this.reviewMnemonic().trim()) { this.actionError.set('Mnemonic is required.'); return; }
    this.busy.set(true);
    this.actionError.set(null);
    this.sldService.engineerReview(this.sldId, {
      approve: true,
      mnemonic: this.reviewMnemonic().trim(),
      substationType: this.reviewSubstationType(),
      rejectionReason: null
    }).subscribe({
      next: () => { this.busy.set(false); this.load(); },
      error: (err) => { this.busy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  submitEngineerReject(): void {
    if (!this.rejectionReason().trim()) { this.actionError.set('A rejection reason is required.'); return; }
    this.runStageAction((id, req) => this.sldService.engineerReview(id, { approve: false, mnemonic: null, substationType: null, rejectionReason: req.rejectionReason }));
  }

  seApprove(): void { this.runStageAction((id) => this.sldService.seReview(id, { approve: true, rejectionReason: null })); }
  seAdjust(): void {
    if (!this.rejectionReason().trim()) { this.actionError.set('A reason is required to request adjustment.'); return; }
    this.runStageAction((id, req) => this.sldService.seReview(id, { approve: false, rejectionReason: req.rejectionReason }));
  }

  dceApprove(): void { this.runStageAction((id) => this.sldService.dceReview(id, { approve: true, rejectionReason: null })); }
  dceAdjust(): void {
    if (!this.rejectionReason().trim()) { this.actionError.set('A reason is required to request adjustment.'); return; }
    this.runStageAction((id, req) => this.sldService.dceReview(id, { approve: false, rejectionReason: req.rejectionReason }));
  }

  requestorApprove(): void { this.runStageAction((id) => this.sldService.requestorApprove(id, { approve: true, rejectionReason: null })); }
  requestorAdjust(): void {
    if (!this.rejectionReason().trim()) { this.actionError.set('A reason is required to request adjustment.'); return; }
    this.runStageAction((id, req) => this.sldService.requestorApprove(id, { approve: false, rejectionReason: req.rejectionReason }));
  }

  private runStageAction(fn: (id: number, req: { rejectionReason: string | null }) => ReturnType<SldService['seReview']>): void {
    this.busy.set(true);
    this.actionError.set(null);
    fn(this.sldId, { rejectionReason: this.rejectionReason() || null }).subscribe({
      next: () => { this.busy.set(false); this.rejectionReason.set(''); this.load(); },
      error: (err) => { this.busy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }
}
