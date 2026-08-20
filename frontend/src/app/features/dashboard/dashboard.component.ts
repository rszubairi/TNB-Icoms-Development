import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { Dashboard } from '../../core/models/dashboard.model';

interface PieSlice {
  label: string;
  value: number;
  color: string;
  path: string;
}

const PIE_COLORS = ['#3b82f6', '#f59e0b', '#8b5cf6', '#10b981', '#ef4444', '#6b7280'];
const BAR_HEIGHT = 200;
const BAR_WIDTH = 560;

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  private dashboardService = inject(DashboardService);

  dashboard = signal<Dashboard | null>(null);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  readonly chartWidth = BAR_WIDTH;
  readonly chartHeight = BAR_HEIGHT;

  weeklyMax = computed(() => Math.max(1, ...((this.dashboard()?.weeklyOutageCounts ?? []).map((w) => w.count))));

  statusPie = computed<PieSlice[]>(() => {
    const items = this.dashboard()?.statusBreakdown ?? [];
    const total = items.reduce((sum, i) => sum + i.count, 0);
    if (total === 0) return [];

    let cumulative = 0;
    return items.map((item, index) => {
      const startAngle = (cumulative / total) * 2 * Math.PI;
      cumulative += item.count;
      const endAngle = (cumulative / total) * 2 * Math.PI;
      return {
        label: item.status,
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
    const r = 85;
    const cx = 95;
    const cy = 95;
    const x1 = cx + r * Math.sin(startAngle);
    const y1 = cy - r * Math.cos(startAngle);
    const x2 = cx + r * Math.sin(endAngle);
    const y2 = cy - r * Math.cos(endAngle);
    const largeArc = endAngle - startAngle > Math.PI ? 1 : 0;
    return `M ${cx} ${cy} L ${x1} ${y1} A ${r} ${r} 0 ${largeArc} 1 ${x2} ${y2} Z`;
  }

  barHeight(count: number): number {
    return Math.round((count / this.weeklyMax()) * (BAR_HEIGHT - 40));
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.dashboardService.get().subscribe({
      next: (dashboard) => { this.dashboard.set(dashboard); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load the dashboard. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }
}
