import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HandoverService } from '../../core/services/handover.service';
import { AuthorisationPersonnelService } from '../../core/services/authorisation-personnel.service';
import { AuthService } from '../../core/services/auth.service';
import { HandoverEntry, HandoverShift, HandoverShiftSummary } from '../../core/models/handover.model';
import { AuthorisationPersonnel } from '../../core/models/authorisation-personnel.model';

@Component({
  selector: 'app-handover',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './handover.component.html',
  styleUrl: './handover.component.css'
})
export class HandoverComponent {
  private handoverService = inject(HandoverService);
  private personnelService = inject(AuthorisationPersonnelService);
  private authService = inject(AuthService);

  zoneId = Number(this.authService.currentUser()?.zoneId ?? 0);

  shiftDate = signal(new Date().toISOString().slice(0, 10));
  shiftType = signal<'Morning' | 'Evening' | 'Night'>('Morning');
  categories = signal<string[]>([]);
  activeCategory = signal<string>('');
  personnel = signal<AuthorisationPersonnel[]>([]);

  shift = signal<HandoverShift | null>(null);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  controlManagerId = signal<number | null>(null);
  switchEngineer1Id = signal<number | null>(null);
  switchEngineer2Id = signal<number | null>(null);
  despatcherId = signal<number | null>(null);
  controlAssistantId = signal<number | null>(null);
  savingControl = signal(false);

  newDescription = signal('');
  newRelatedOutageId = signal<number | null>(null);
  addingEntry = signal(false);
  passing = signal(false);

  history = signal<HandoverShiftSummary[]>([]);
  showHistory = signal(false);

  entriesForActiveCategory = computed(() =>
    (this.shift()?.entries ?? []).filter((e) => e.category === this.activeCategory())
  );

  constructor() {
    this.handoverService.listCategories().subscribe({
      next: (r) => {
        this.categories.set(r.categories);
        this.activeCategory.set(r.categories[0] ?? '');
      },
      error: () => {}
    });
    this.personnelService.list().subscribe({ next: (p) => this.personnel.set(p.filter((x) => x.isActive)), error: () => {} });
    this.load();
  }

  private load(): void {
    if (!this.zoneId) {
      this.errorMessage.set('No zone assigned to your account.');
      this.loading.set(false);
      return;
    }
    this.loading.set(true);
    this.errorMessage.set(null);
    this.handoverService.getOrCreateShift(this.shiftDate(), this.shiftType(), this.zoneId).subscribe({
      next: (shift) => {
        this.shift.set(shift);
        this.controlManagerId.set(shift.controlManagerId);
        this.switchEngineer1Id.set(shift.switchEngineer1Id);
        this.switchEngineer2Id.set(shift.switchEngineer2Id);
        this.despatcherId.set(shift.despatcherId);
        this.controlAssistantId.set(shift.controlAssistantId);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load this shift. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  onShiftDateChange(value: string): void { this.shiftDate.set(value); this.load(); }
  onShiftTypeChange(value: 'Morning' | 'Evening' | 'Night'): void { this.shiftType.set(value); this.load(); }

  saveControl(): void {
    const shift = this.shift();
    if (!shift) return;
    this.savingControl.set(true);
    this.handoverService.updateShiftControl(shift.shiftId, {
      controlManagerId: this.controlManagerId(),
      switchEngineer1Id: this.switchEngineer1Id(),
      switchEngineer2Id: this.switchEngineer2Id(),
      despatcherId: this.despatcherId(),
      controlAssistantId: this.controlAssistantId()
    }).subscribe({
      next: () => { this.savingControl.set(false); this.load(); },
      error: (err) => { this.savingControl.set(false); this.errorMessage.set(err?.error?.error ?? 'Unable to save control team.'); }
    });
  }

  addEntry(): void {
    const shift = this.shift();
    if (!shift || !this.newDescription().trim()) return;
    this.addingEntry.set(true);
    this.handoverService.addEntry(shift.shiftId, {
      category: this.activeCategory(),
      description: this.newDescription().trim(),
      relatedOutageId: this.newRelatedOutageId()
    }).subscribe({
      next: () => {
        this.addingEntry.set(false);
        this.newDescription.set('');
        this.newRelatedOutageId.set(null);
        this.load();
      },
      error: (err) => { this.addingEntry.set(false); this.errorMessage.set(err?.error?.error ?? 'Unable to add entry.'); }
    });
  }

  removeEntry(entry: HandoverEntry): void {
    if (!confirm('Remove this entry?')) return;
    this.handoverService.deleteEntry(entry.handoverEntryId).subscribe({
      next: () => this.load(),
      error: (err) => this.errorMessage.set(err?.error?.error ?? 'Unable to remove entry.')
    });
  }

  passHandover(): void {
    const shift = this.shift();
    if (!shift) return;
    if (!confirm(`Pass this ${shift.shiftType} shift's handover to the next shift? This locks it.`)) return;
    this.passing.set(true);
    this.handoverService.passHandover(shift.shiftId).subscribe({
      next: () => { this.passing.set(false); this.load(); },
      error: (err) => { this.passing.set(false); this.errorMessage.set(err?.error?.error ?? 'Unable to pass handover.'); }
    });
  }

  toggleHistory(): void {
    this.showHistory.update((v) => !v);
    if (this.showHistory() && this.zoneId) {
      this.handoverService.listShifts(this.zoneId).subscribe({ next: (h) => this.history.set(h), error: () => {} });
    }
  }

  openHistoryShift(summary: HandoverShiftSummary): void {
    this.shiftDate.set(summary.shiftDate.slice(0, 10));
    this.shiftType.set(summary.shiftType as 'Morning' | 'Evening' | 'Night');
    this.showHistory.set(false);
    this.load();
  }
}
