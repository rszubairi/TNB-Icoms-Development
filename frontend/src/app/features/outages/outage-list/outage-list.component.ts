import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OutageService } from '../../../core/services/outage.service';
import { OutageListItem } from '../../../core/models/outage.model';

export type OutageListMode = 'pendingReview' | 'confirmation' | 'pendingApproval' | 'repository';

interface ModeConfig {
  title: string;
  subtitle: string;
  primaryLabel: string;
  secondaryLabel: string | null;
  readOnly: boolean;
}

const MODE_CONFIG: Record<OutageListMode, ModeConfig> = {
  pendingReview: {
    title: 'Outage Pending Review',
    subtitle: 'Outages awaiting Planner agreement.',
    primaryLabel: 'Agree',
    secondaryLabel: 'Disagree',
    readOnly: false
  },
  confirmation: {
    title: 'Confirmation Page',
    subtitle: 'Outages agreed by the Planner, awaiting your confirmation before they go to GNM.',
    primaryLabel: 'Confirm',
    secondaryLabel: 'Reject',
    readOnly: false
  },
  pendingApproval: {
    title: 'Outage Pending Approval',
    subtitle: 'Agreed and confirmed outages waiting for GNM approval.',
    primaryLabel: 'Approve',
    secondaryLabel: 'Disapprove',
    readOnly: false
  },
  repository: {
    title: 'Data Repository',
    subtitle: 'All outages that have been Agreed and Confirmed, within your zone.',
    primaryLabel: '',
    secondaryLabel: null,
    readOnly: true
  }
};

@Component({
  selector: 'app-outage-list',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './outage-list.component.html',
  styleUrl: './outage-list.component.css'
})
export class OutageListComponent {
  private outageService = inject(OutageService);
  private route = inject(ActivatedRoute);

  mode = signal<OutageListMode>((this.route.snapshot.data['mode'] as OutageListMode) ?? 'repository');
  config = computed(() => MODE_CONFIG[this.mode()]);

  outages = signal<OutageListItem[]>([]);
  selectedIds = signal<Set<number>>(new Set());
  loading = signal(true);
  errorMessage = signal<string | null>(null);
  actionError = signal<string | null>(null);
  processing = signal(false);
  rowBusyId = signal<number | null>(null);

  allChecked = computed(() => this.outages().length > 0 && this.selectedIds().size === this.outages().length);

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.selectedIds.set(new Set());

    const filter = {
      pendingPlannerReviewOnly: this.mode() === 'pendingReview',
      pendingConfirmationOnly: this.mode() === 'confirmation',
      pendingGnmApprovalOnly: this.mode() === 'pendingApproval',
      agreedAndConfirmedOnly: this.mode() === 'repository'
    };

    this.outageService.list(filter).subscribe({
      next: (outages) => {
        this.outages.set(outages);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load outages. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  toggleAll(checked: boolean): void {
    this.selectedIds.set(checked ? new Set(this.outages().map((o) => o.outageId)) : new Set());
  }

  toggleOne(id: number, checked: boolean): void {
    this.selectedIds.update((set) => {
      const next = new Set(set);
      if (checked) next.add(id);
      else next.delete(id);
      return next;
    });
  }

  isChecked(id: number): boolean {
    return this.selectedIds().has(id);
  }

  // --- Single-row actions ---

  primaryAction(outage: OutageListItem): void {
    this.rowBusyId.set(outage.outageId);
    const request$ =
      this.mode() === 'pendingReview' ? this.outageService.agree(outage.outageId) :
      this.mode() === 'pendingApproval' ? this.outageService.gnmApprove(outage.outageId) :
      this.outageService.confirm(outage.outageId);
    request$.subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.actionError.set(err?.error?.error ?? err?.message ?? 'Action failed.');
      }
    });
  }

  secondaryAction(outage: OutageListItem): void {
    const isReject = this.mode() === 'confirmation';
    if (isReject && !confirm(`Reject and delete outage ${outage.outageNumber}? This cannot be undone.`)) return;
    if (this.mode() === 'pendingApproval' && !confirm(`Disapprove outage ${outage.outageNumber}?`)) return;

    this.rowBusyId.set(outage.outageId);
    const request$ =
      this.mode() === 'pendingReview' ? this.outageService.disagree(outage.outageId) :
      this.mode() === 'pendingApproval' ? this.outageService.gnmDisapprove(outage.outageId) :
      this.outageService.reject(outage.outageId);
    request$.subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.actionError.set(err?.error?.error ?? err?.message ?? 'Action failed.');
      }
    });
  }

  // --- Bulk actions ---

  bulkPrimary(): void {
    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) return;

    this.processing.set(true);
    this.actionError.set(null);
    const request$ =
      this.mode() === 'pendingReview' ? this.outageService.bulkAgree(ids) :
      this.mode() === 'pendingApproval' ? this.outageService.bulkGnmApprove(ids) :
      this.outageService.bulkConfirm(ids);
    request$.subscribe({
      next: (result) => {
        this.processing.set(false);
        if (result.errors.length > 0) this.actionError.set(result.errors.join('; '));
        this.load();
      },
      error: (err) => {
        this.processing.set(false);
        this.actionError.set(err?.error?.error ?? err?.message ?? 'Bulk action failed.');
      }
    });
  }

  bulkSecondary(): void {
    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) return;

    const isReject = this.mode() === 'confirmation';
    if (isReject && !confirm(`Reject and delete ${ids.length} outage(s)? This cannot be undone.`)) return;
    if (this.mode() === 'pendingApproval' && !confirm(`Disapprove ${ids.length} outage(s)?`)) return;

    this.processing.set(true);
    this.actionError.set(null);
    const request$ =
      this.mode() === 'pendingReview' ? this.outageService.bulkDisagree(ids) :
      this.mode() === 'pendingApproval' ? this.outageService.bulkGnmDisapprove(ids) :
      this.outageService.bulkReject(ids);
    request$.subscribe({
      next: (result) => {
        this.processing.set(false);
        if (result.errors.length > 0) this.actionError.set(result.errors.join('; '));
        this.load();
      },
      error: (err) => {
        this.processing.set(false);
        this.actionError.set(err?.error?.error ?? err?.message ?? 'Bulk action failed.');
      }
    });
  }
}
