import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HealthCheckResult, HealthCheckService } from '../../core/services/health-check.service';

interface HealthCheckGroup {
  name: string;
  checks: HealthCheckResult[];
  passed: number;
  failed: number;
  total: number;
}

@Component({
  selector: 'app-health-check',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './health-check.component.html',
  styleUrl: './health-check.component.css'
})
export class HealthCheckComponent {
  private healthCheckService = inject(HealthCheckService);

  results = this.healthCheckService.results;
  running = this.healthCheckService.running;
  lastRunAt = signal<Date | null>(null);

  total = computed(() => this.results().length);
  done = computed(() => this.results().filter((r) => r.status === 'passed' || r.status === 'failed').length);
  passed = computed(() => this.results().filter((r) => r.status === 'passed').length);
  failed = computed(() => this.results().filter((r) => r.status === 'failed').length);

  allDone = computed(() => this.total() > 0 && this.done() === this.total());
  overallStatus = computed<'idle' | 'running' | 'healthy' | 'degraded'>(() => {
    if (this.total() === 0) return 'idle';
    if (!this.allDone()) return 'running';
    return this.failed() === 0 ? 'healthy' : 'degraded';
  });

  groups = computed<HealthCheckGroup[]>(() => {
    const byGroup = new Map<string, HealthCheckResult[]>();
    for (const result of this.results()) {
      if (!byGroup.has(result.group)) byGroup.set(result.group, []);
      byGroup.get(result.group)!.push(result);
    }
    return Array.from(byGroup.entries()).map(([name, checks]) => ({
      name,
      checks,
      passed: checks.filter((c) => c.status === 'passed').length,
      failed: checks.filter((c) => c.status === 'failed').length,
      total: checks.length
    }));
  });

  failedChecks = computed(() => this.results().filter((r) => r.status === 'failed'));

  constructor() {
    this.run();
  }

  run(): void {
    this.healthCheckService.runAll().subscribe({
      complete: () => this.lastRunAt.set(new Date())
    });
  }
}
