import { Component, inject, signal } from '@angular/core';
import { OutageScheduleWindowService } from '../../../core/services/outage-schedule-window.service';
import { OutageScheduleWindow } from '../../../core/models/outage-schedule-window.model';

const MONTHS = ['JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC'];
const OUTAGE_TYPES = ['Planned', 'Unplanned', 'Emergency', 'Forced'];
const WORK_TYPES = ['Live', 'Dead'];

function key(workType: string, outageType: string, month: number): string {
  return `${workType}::${outageType}::${month}`;
}

@Component({
  selector: 'app-outage-scheduling',
  standalone: true,
  imports: [],
  templateUrl: './outage-scheduling.component.html',
  styleUrl: './outage-scheduling.component.css'
})
export class OutageSchedulingComponent {
  private scheduleService = inject(OutageScheduleWindowService);

  months = MONTHS;
  outageTypes = OUTAGE_TYPES;
  workTypes = WORK_TYPES;

  loading = signal(true);
  errorMessage = signal<string | null>(null);
  saving = signal(false);
  saveMessage = signal<string | null>(null);

  private allowed = signal<Record<string, boolean>>({});

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.scheduleService.list().subscribe({
      next: (windows) => {
        const map: Record<string, boolean> = {};
        for (const w of windows) {
          map[key(w.workTypeCode, w.outageTypeCode, w.month)] = w.isAllowed;
        }
        this.allowed.set(map);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load outage scheduling. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  isAllowed(workType: string, outageType: string, month: number): boolean {
    return this.allowed()[key(workType, outageType, month)] ?? false;
  }

  toggle(workType: string, outageType: string, month: number): void {
    this.allowed.update((map) => {
      const k = key(workType, outageType, month);
      return { ...map, [k]: !map[k] };
    });
    this.saveMessage.set(null);
  }

  save(): void {
    const windows: OutageScheduleWindow[] = [];
    for (const workType of this.workTypes) {
      for (const outageType of this.outageTypes) {
        for (let month = 1; month <= 12; month++) {
          windows.push({
            workTypeCode: workType,
            outageTypeCode: outageType,
            month,
            isAllowed: this.isAllowed(workType, outageType, month)
          });
        }
      }
    }

    this.saving.set(true);
    this.saveMessage.set(null);
    this.scheduleService.save(windows).subscribe({
      next: () => {
        this.saving.set(false);
        this.saveMessage.set('Changes saved.');
      },
      error: (err) => {
        this.saving.set(false);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to save changes.');
      }
    });
  }
}
