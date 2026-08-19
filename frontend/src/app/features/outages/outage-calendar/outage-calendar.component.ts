import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OutageService } from '../../../core/services/outage.service';
import { OutageListItem } from '../../../core/models/outage.model';

interface CalendarDay {
  date: Date;
  inCurrentMonth: boolean;
  isToday: boolean;
  outages: OutageListItem[];
}

const STATUS_CLASS: Record<string, string> = {
  Pending: 'chip-pending',
  Agreed: 'chip-agreed',
  Confirmed: 'chip-confirmed',
  Approved: 'chip-approved',
  Rejected: 'chip-rejected',
  Closed: 'chip-closed'
};

@Component({
  selector: 'app-outage-calendar',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './outage-calendar.component.html',
  styleUrl: './outage-calendar.component.css'
})
export class OutageCalendarComponent {
  private outageService = inject(OutageService);

  anchorDate = signal(new Date());
  outages = signal<OutageListItem[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);
  selectedDay = signal<CalendarDay | null>(null);

  monthLabel = computed(() =>
    this.anchorDate().toLocaleDateString('en-GB', { month: 'long', year: 'numeric' })
  );

  weeks = computed<CalendarDay[][]>(() => {
    const anchor = this.anchorDate();
    const monthStart = new Date(anchor.getFullYear(), anchor.getMonth(), 1);
    const gridStart = new Date(monthStart);
    const startOffset = (monthStart.getDay() + 6) % 7; // Monday-first
    gridStart.setDate(monthStart.getDate() - startOffset);

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const days: CalendarDay[] = [];
    for (let i = 0; i < 35; i++) {
      const date = new Date(gridStart);
      date.setDate(gridStart.getDate() + i);
      const dayStart = new Date(date.getFullYear(), date.getMonth(), date.getDate());
      const dayEnd = new Date(dayStart.getFullYear(), dayStart.getMonth(), dayStart.getDate() + 1);

      const dayOutages = this.outages().filter((o) => {
        const start = new Date(o.plannedStartAt);
        const end = new Date(o.plannedEndAt);
        return start < dayEnd && dayStart < end;
      });

      days.push({
        date,
        inCurrentMonth: date.getMonth() === anchor.getMonth(),
        isToday: dayStart.getTime() === today.getTime(),
        outages: dayOutages
      });
    }

    const weeks: CalendarDay[][] = [];
    for (let i = 0; i < 35; i += 7) weeks.push(days.slice(i, i + 7));
    return weeks;
  });

  constructor() {
    this.load();
  }

  private rangeBounds(): { rangeStart: string; rangeEnd: string } {
    const anchor = this.anchorDate();
    const monthStart = new Date(anchor.getFullYear(), anchor.getMonth(), 1);
    const startOffset = (monthStart.getDay() + 6) % 7;
    const gridStart = new Date(monthStart);
    gridStart.setDate(monthStart.getDate() - startOffset);
    const gridEnd = new Date(gridStart);
    gridEnd.setDate(gridStart.getDate() + 35);
    return { rangeStart: gridStart.toISOString(), rangeEnd: gridEnd.toISOString() };
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.selectedDay.set(null);

    this.outageService.list(this.rangeBounds()).subscribe({
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

  prevMonth(): void {
    const a = this.anchorDate();
    this.anchorDate.set(new Date(a.getFullYear(), a.getMonth() - 1, 1));
    this.load();
  }

  nextMonth(): void {
    const a = this.anchorDate();
    this.anchorDate.set(new Date(a.getFullYear(), a.getMonth() + 1, 1));
    this.load();
  }

  today(): void {
    this.anchorDate.set(new Date());
    this.load();
  }

  selectDay(day: CalendarDay): void {
    this.selectedDay.set(day.outages.length > 0 ? day : null);
  }

  statusClass(status: string): string {
    return STATUS_CLASS[status] ?? 'chip-pending';
  }
}
