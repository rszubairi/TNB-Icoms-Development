import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmailLogService } from '../../../core/services/email-log.service';
import { EmailLog } from '../../../core/models/email-log.model';

@Component({
  selector: 'app-email-logs',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './email-logs.component.html',
  styleUrl: './email-logs.component.css'
})
export class EmailLogsComponent {
  private emailLogService = inject(EmailLogService);

  logs = signal<EmailLog[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  statusFilter = signal<string>('');
  toAddressFilter = signal<string>('');

  expandedId = signal<number | null>(null);

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.emailLogService.list(this.statusFilter() || undefined, undefined, this.toAddressFilter() || undefined).subscribe({
      next: (logs) => { this.logs.set(logs); this.loading.set(false); },
      error: () => { this.errorMessage.set('Unable to load email logs. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  onStatusChange(value: string): void { this.statusFilter.set(value); this.load(); }
  onToAddressChange(value: string): void { this.toAddressFilter.set(value); this.load(); }

  toggle(log: EmailLog): void {
    this.expandedId.set(this.expandedId() === log.emailLogId ? null : log.emailLogId);
  }
}
