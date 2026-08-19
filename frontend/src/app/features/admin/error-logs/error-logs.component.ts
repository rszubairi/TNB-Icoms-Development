import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ErrorLogService } from '../../../core/services/error-log.service';
import { ErrorLog } from '../../../core/models/error-log.model';

@Component({
  selector: 'app-error-logs',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './error-logs.component.html',
  styleUrl: './error-logs.component.css'
})
export class ErrorLogsComponent {
  private errorLogService = inject(ErrorLogService);

  logs = signal<ErrorLog[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  sourceFilter = signal<string>('');
  severityFilter = signal<string>('');

  expandedId = signal<number | null>(null);

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.errorLogService.list(this.sourceFilter() || undefined, this.severityFilter() || undefined).subscribe({
      next: (logs) => { this.logs.set(logs); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load error logs. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  onSourceChange(value: string): void { this.sourceFilter.set(value); this.load(); }
  onSeverityChange(value: string): void { this.severityFilter.set(value); this.load(); }

  toggle(log: ErrorLog): void {
    this.expandedId.set(this.expandedId() === log.errorLogId ? null : log.errorLogId);
  }
}
