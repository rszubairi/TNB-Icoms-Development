import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ChangeRequestService } from '../../../core/services/change-request.service';
import { ChangeRequestBatch } from '../../../core/models/change-request.model';

@Component({
  selector: 'app-change-request-review',
  standalone: true,
  imports: [DatePipe, RouterLink, FormsModule],
  templateUrl: './change-request-review.component.html',
  styleUrl: './change-request-review.component.css'
})
export class ChangeRequestReviewComponent {
  private changeRequestService = inject(ChangeRequestService);

  batches = signal<ChangeRequestBatch[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);
  busyBatchId = signal<string | null>(null);

  rejectingBatchId = signal<string | null>(null);
  rejectComment = signal('');

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.changeRequestService.listPending().subscribe({
      next: (batches) => {
        this.batches.set(batches);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load pending change requests. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  approve(batch: ChangeRequestBatch): void {
    if (!confirm(`Approve this change request for outage ${batch.outageNumber}? Changes apply immediately.`)) return;
    this.busyBatchId.set(batch.batchId);
    this.changeRequestService.approve(batch.batchId).subscribe({
      next: () => {
        this.busyBatchId.set(null);
        this.load();
      },
      error: (err) => {
        this.busyBatchId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to approve.');
      }
    });
  }

  startReject(batch: ChangeRequestBatch): void {
    this.rejectingBatchId.set(batch.batchId);
    this.rejectComment.set('');
  }

  cancelReject(): void {
    this.rejectingBatchId.set(null);
  }

  confirmReject(batch: ChangeRequestBatch): void {
    this.busyBatchId.set(batch.batchId);
    this.changeRequestService.reject(batch.batchId, this.rejectComment().trim()).subscribe({
      next: () => {
        this.busyBatchId.set(null);
        this.rejectingBatchId.set(null);
        this.load();
      },
      error: (err) => {
        this.busyBatchId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to reject.');
      }
    });
  }
}
