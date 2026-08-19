import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommissioningMemoService } from '../../../core/services/commissioning-memo.service';
import { CommissioningMemoDetail } from '../../../core/models/commissioning-memo.model';

@Component({
  selector: 'app-memo-detail',
  standalone: true,
  imports: [DatePipe, FormsModule, RouterLink],
  templateUrl: './memo-detail.component.html',
  styleUrl: './memo-detail.component.css'
})
export class MemoDetailComponent {
  private route = inject(ActivatedRoute);
  private memoService = inject(CommissioningMemoService);

  memoId = Number(this.route.snapshot.paramMap.get('id'));

  memo = signal<CommissioningMemoDetail | null>(null);
  loading = signal(true);
  errorMessage = signal<string | null>(null);
  actionError = signal<string | null>(null);
  busy = signal(false);

  rejectionReason = signal('');
  commissioningResult = signal('InProgress');

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.memoService.getById(this.memoId).subscribe({
      next: (memo) => { this.memo.set(memo); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load this memo. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  downloadCoverPage(): void {
    this.memoService.downloadCoverPage(this.memoId).subscribe({
      next: (blob) => this.memoService.triggerDownload(blob, `${this.memo()?.memoNo ?? this.memoId}.pdf`),
      error: () => this.actionError.set('Unable to download cover page.')
    });
  }

  private runStage(fn: (id: number, req: { approve: boolean; rejectionReason: string | null }) => ReturnType<CommissioningMemoService['seReview']>, approve: boolean): void {
    if (!approve && !this.rejectionReason().trim()) {
      this.actionError.set('A rejection reason is required.');
      return;
    }
    this.busy.set(true);
    this.actionError.set(null);
    fn(this.memoId, { approve, rejectionReason: approve ? null : this.rejectionReason() }).subscribe({
      next: () => { this.busy.set(false); this.rejectionReason.set(''); this.load(); },
      error: (err) => { this.busy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  engineerPicApprove(): void { this.runStage((id, r) => this.memoService.engineerPicReview(id, r), true); }
  engineerPicReject(): void { this.runStage((id, r) => this.memoService.engineerPicReview(id, r), false); }
  seApprove(): void { this.runStage((id, r) => this.memoService.seReview(id, r), true); }
  seReject(): void { this.runStage((id, r) => this.memoService.seReview(id, r), false); }
  dceApprove(): void { this.runStage((id, r) => this.memoService.dceReview(id, r), true); }
  dceReject(): void { this.runStage((id, r) => this.memoService.dceReview(id, r), false); }
  ceGnmApprove(): void { this.runStage((id, r) => this.memoService.ceGnmReview(id, r), true); }
  ceGnmReject(): void { this.runStage((id, r) => this.memoService.ceGnmReview(id, r), false); }
  finalApprove(): void { this.runStage((id, r) => this.memoService.finalSignOff(id, r), true); }
  finalReject(): void { this.runStage((id, r) => this.memoService.finalSignOff(id, r), false); }

  setResult(): void {
    this.busy.set(true);
    this.actionError.set(null);
    this.memoService.setCommissioningResult(this.memoId, { commissioningResult: this.commissioningResult() }).subscribe({
      next: () => { this.busy.set(false); this.load(); },
      error: (err) => { this.busy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }
}
