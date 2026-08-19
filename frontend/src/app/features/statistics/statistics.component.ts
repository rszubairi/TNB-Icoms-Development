import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StatisticsService } from '../../core/services/statistics.service';
import { StatisticsDashboard } from '../../core/models/statistics.model';

interface BarDatum {
  label: string;
  value: number;
}

interface PieSlice {
  label: string;
  value: number;
  color: string;
  path: string;
}

const PIE_COLORS = ['#3b82f6', '#f59e0b', '#ef4444', '#10b981', '#8b5cf6'];
const BAR_HEIGHT = 220;
const BAR_WIDTH = 480;

@Component({
  selector: 'app-statistics',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './statistics.component.html',
  styleUrl: './statistics.component.css'
})
export class StatisticsComponent {
  private statisticsService = inject(StatisticsService);

  years: number[] = Array.from({ length: 6 }, (_, i) => new Date().getFullYear() - i);
  months = [
    { value: 1, label: 'January' }, { value: 2, label: 'February' }, { value: 3, label: 'March' },
    { value: 4, label: 'April' }, { value: 5, label: 'May' }, { value: 6, label: 'June' },
    { value: 7, label: 'July' }, { value: 8, label: 'August' }, { value: 9, label: 'September' },
    { value: 10, label: 'October' }, { value: 11, label: 'November' }, { value: 12, label: 'December' }
  ];

  selectedYear = signal(new Date().getFullYear());
  selectedMonth = signal<number | null>(new Date().getMonth() + 1);

  loading = signal(true);
  errorMessage = signal<string | null>(null);
  dashboard = signal<StatisticsDashboard | null>(null);

  approvedBars = computed<BarDatum[]>(() =>
    (this.dashboard()?.approvedOutagesByDepartment ?? []).map((d) => ({ label: d.department, value: d.count }))
  );

  routineBars = computed(() => this.dashboard()?.routineMaintenanceByDepartment ?? []);

  typePie = computed<PieSlice[]>(() => {
    const items = this.dashboard()?.outageTypeBreakdown ?? [];
    const total = items.reduce((sum, i) => sum + i.count, 0);
    if (total === 0) return [];

    let cumulative = 0;
    return items.map((item, index) => {
      const startAngle = (cumulative / total) * 2 * Math.PI;
      cumulative += item.count;
      const endAngle = (cumulative / total) * 2 * Math.PI;
      return {
        label: item.outageTypeCode,
        value: item.count,
        color: PIE_COLORS[index % PIE_COLORS.length],
        path: this.arcPath(startAngle, endAngle)
      };
    });
  });

  constructor() {
    this.load();
  }

  private arcPath(startAngle: number, endAngle: number): string {
    const r = 90;
    const cx = 100;
    const cy = 100;
    const x1 = cx + r * Math.sin(startAngle);
    const y1 = cy - r * Math.cos(startAngle);
    const x2 = cx + r * Math.sin(endAngle);
    const y2 = cy - r * Math.cos(endAngle);
    const largeArc = endAngle - startAngle > Math.PI ? 1 : 0;
    return `M ${cx} ${cy} L ${x1} ${y1} A ${r} ${r} 0 ${largeArc} 1 ${x2} ${y2} Z`;
  }

  barHeight(value: number, max: number): number {
    if (max === 0) return 0;
    return Math.round((value / max) * (BAR_HEIGHT - 40));
  }

  maxValue(items: BarDatum[]): number {
    return Math.max(1, ...items.map((i) => i.value));
  }

  maxStacked(items: { completed: number; pending: number }[]): number {
    return Math.max(1, ...items.map((i) => i.completed + i.pending));
  }

  readonly chartWidth = BAR_WIDTH;
  readonly chartHeight = BAR_HEIGHT;

  onYearChange(value: string): void {
    this.selectedYear.set(Number(value));
    this.load();
  }

  onMonthChange(value: string): void {
    this.selectedMonth.set(value ? Number(value) : null);
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.statisticsService.getDashboard(this.selectedYear(), this.selectedMonth()).subscribe({
      next: (data) => {
        this.dashboard.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load statistics. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }
}
