import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GncService } from '../../../core/services/gnc.service';
import { GncOutageListItem } from '../../../core/models/gnc.model';

type GncActiveMode = 'active' | 'authorisationInForce';
type ActionMode = 'complete' | 'extend';

@Component({
  selector: 'app-gnc-active',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './gnc-active.component.html',
  styleUrl: './gnc-active.component.css'
})
export class GncActiveComponent {
  private gncService = inject(GncService);
  private route = inject(ActivatedRoute);

  mode = signal<GncActiveMode>((this.route.snapshot.data['mode'] as GncActiveMode) ?? 'active');
  title = () => (this.mode() === 'authorisationInForce' ? 'Authorisation in Force' : 'Active Outages');
  subtitle = () =>
    this.mode() === 'authorisationInForce'
      ? 'Outages currently Taken-Active, colour-coded by whether they are within their scheduled window.'
      : 'Outages holding a live authorisation (Taken-Active or Taken-Completed). Complete or extend as needed.';

  outages = signal<GncOutageListItem[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  activeOutage = signal<GncOutageListItem | null>(null);
  actionMode = signal<ActionMode | null>(null);
  actionBusy = signal(false);
  actionError = signal<string | null>(null);

  takenCompletedAt = signal(this.nowLocal());
  extendedTo = signal(this.nowLocal());
  remark = signal('');

  constructor() {
    this.load();
  }

  private nowLocal(): string {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    const request$ = this.mode() === 'authorisationInForce' ? this.gncService.listAuthorisationInForce() : this.gncService.listActive();
    request$.subscribe({
      next: (outages) => { this.outages.set(outages); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load outages. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  timelineClass(outage: GncOutageListItem): string {
    if (outage.gncStatus !== 'Taken-Active') return '';
    return outage.timelineStatus === 'Overdue' ? 'status-overdue' : outage.timelineStatus === 'Extended' ? 'status-extended' : 'status-ontime';
  }

  openAction(outage: GncOutageListItem, mode: ActionMode): void {
    this.activeOutage.set(outage);
    this.actionMode.set(mode);
    this.actionError.set(null);
    this.takenCompletedAt.set(this.nowLocal());
    this.extendedTo.set(this.nowLocal());
    this.remark.set('');
  }

  closeAction(): void {
    this.activeOutage.set(null);
    this.actionMode.set(null);
  }

  submitComplete(): void {
    const outage = this.activeOutage();
    if (!outage) return;
    this.actionBusy.set(true);
    this.actionError.set(null);
    this.gncService.complete(outage.outageId, { takenCompletedAt: this.takenCompletedAt(), remark: this.remark() || null }).subscribe({
      next: () => { this.actionBusy.set(false); this.closeAction(); this.load(); },
      error: (err) => { this.actionBusy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  submitExtend(): void {
    const outage = this.activeOutage();
    if (!outage) return;
    this.actionBusy.set(true);
    this.actionError.set(null);
    this.gncService.extend(outage.outageId, { extendedTo: this.extendedTo(), remark: this.remark() || null }).subscribe({
      next: () => { this.actionBusy.set(false); this.closeAction(); this.load(); },
      error: (err) => { this.actionBusy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  close(outage: GncOutageListItem): void {
    if (!confirm(`Close outage ${outage.outageNumber}? This finalises it as Outage Closed.`)) return;
    this.gncService.close(outage.outageId).subscribe({
      next: () => this.load(),
      error: (err) => this.errorMessage.set(err?.error?.error ?? 'Unable to close outage.')
    });
  }
}
