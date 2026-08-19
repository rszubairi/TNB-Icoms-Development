import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GncService } from '../../../core/services/gnc.service';
import { AuthorisationPersonnelService } from '../../../core/services/authorisation-personnel.service';
import { DropdownValueService } from '../../../core/services/dropdown-value.service';
import { GncOutageListItem } from '../../../core/models/gnc.model';
import { AuthorisationPersonnel } from '../../../core/models/authorisation-personnel.model';
import { DropdownValue } from '../../../core/models/dropdown-value.model';

type ActionMode = 'take-active' | 'not-taken' | 'cancel';

@Component({
  selector: 'app-gnc-scheduled',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './gnc-scheduled.component.html',
  styleUrl: './gnc-scheduled.component.css'
})
export class GncScheduledComponent {
  private gncService = inject(GncService);
  private personnelService = inject(AuthorisationPersonnelService);
  private dropdownService = inject(DropdownValueService);

  outages = signal<GncOutageListItem[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  personnel = signal<AuthorisationPersonnel[]>([]);
  notTakenReasons = signal<DropdownValue[]>([]);

  activeOutage = signal<GncOutageListItem | null>(null);
  actionMode = signal<ActionMode | null>(null);
  actionBusy = signal(false);
  actionError = signal<string | null>(null);

  authorisationNo = signal('');
  personnelId = signal<number | null>(null);
  takenActiveAt = signal(this.nowLocal());
  remark = signal('');
  notTakenReasonId = signal<number | null>(null);

  constructor() {
    this.load();
    this.personnelService.list().subscribe({ next: (p) => this.personnel.set(p.filter((x) => x.isActive)), error: () => {} });
    this.dropdownService.listByCategory('NotTakenReason').subscribe({ next: (r) => this.notTakenReasons.set(r), error: () => {} });
  }

  private nowLocal(): string {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.gncService.listScheduled().subscribe({
      next: (outages) => { this.outages.set(outages); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load scheduled outages. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  openAction(outage: GncOutageListItem, mode: ActionMode): void {
    this.activeOutage.set(outage);
    this.actionMode.set(mode);
    this.actionError.set(null);
    this.authorisationNo.set('');
    this.personnelId.set(null);
    this.takenActiveAt.set(this.nowLocal());
    this.remark.set('');
    this.notTakenReasonId.set(null);
  }

  closeAction(): void {
    this.activeOutage.set(null);
    this.actionMode.set(null);
  }

  submitTakeActive(): void {
    const outage = this.activeOutage();
    if (!outage) return;
    if (!this.authorisationNo().trim() || !this.personnelId()) {
      this.actionError.set('Authorisation No. and Authoriser are required.');
      return;
    }
    this.actionBusy.set(true);
    this.actionError.set(null);
    this.gncService.takeActive(outage.outageId, {
      authorisationNo: this.authorisationNo().trim(),
      personnelId: this.personnelId()!,
      takenActiveAt: this.takenActiveAt(),
      remark: this.remark() || null
    }).subscribe({
      next: () => { this.actionBusy.set(false); this.closeAction(); this.load(); },
      error: (err) => { this.actionBusy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  submitNotTaken(): void {
    const outage = this.activeOutage();
    if (!outage) return;
    if (!this.notTakenReasonId()) {
      this.actionError.set('A reason is required.');
      return;
    }
    this.actionBusy.set(true);
    this.actionError.set(null);
    this.gncService.notTaken(outage.outageId, { notTakenReasonId: this.notTakenReasonId()!, remark: this.remark() || null }).subscribe({
      next: () => { this.actionBusy.set(false); this.closeAction(); this.load(); },
      error: (err) => { this.actionBusy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }

  submitCancel(): void {
    const outage = this.activeOutage();
    if (!outage) return;
    this.actionBusy.set(true);
    this.actionError.set(null);
    this.gncService.cancel(outage.outageId, { remark: this.remark() || null }).subscribe({
      next: () => { this.actionBusy.set(false); this.closeAction(); this.load(); },
      error: (err) => { this.actionBusy.set(false); this.actionError.set(err?.error?.error ?? 'Action failed.'); }
    });
  }
}
