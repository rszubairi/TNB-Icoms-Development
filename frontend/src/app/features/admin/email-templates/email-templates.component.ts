import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmailTemplateService } from '../../../core/services/email-template.service';
import { EmailTemplate } from '../../../core/models/email-template.model';

@Component({
  selector: 'app-email-templates',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './email-templates.component.html',
  styleUrl: './email-templates.component.css'
})
export class EmailTemplatesComponent {
  private emailTemplateService = inject(EmailTemplateService);

  templates = signal<EmailTemplate[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  selected = signal<EmailTemplate | null>(null);
  editSubject = signal('');
  editBody = signal('');
  editIsActive = signal(true);
  saveError = signal<string | null>(null);
  saving = signal(false);

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.emailTemplateService.list().subscribe({
      next: (templates) => {
        this.templates.set(templates);
        this.loading.set(false);
        if (!this.selected() && templates.length > 0) this.select(templates[0]);
      },
      error: () => { this.errorMessage.set('Unable to load email templates. The backend API may not be running yet.'); this.loading.set(false); }
    });
  }

  select(template: EmailTemplate): void {
    this.selected.set(template);
    this.editSubject.set(template.subject);
    this.editBody.set(template.bodyHtml);
    this.editIsActive.set(template.isActive);
    this.saveError.set(null);
  }

  tagList(template: EmailTemplate | null): string[] {
    if (!template?.availableTags) return [];
    return template.availableTags.split(',').map((t) => t.trim()).filter(Boolean);
  }

  insertTag(tag: string): void {
    this.editBody.set(this.editBody() + `{{${tag}}}`);
  }

  save(): void {
    const template = this.selected();
    if (!template) return;
    if (!this.editSubject().trim() || !this.editBody().trim()) {
      this.saveError.set('Subject and Body are required.');
      return;
    }

    this.saving.set(true);
    this.saveError.set(null);
    this.emailTemplateService.update(template.templateCode, {
      subject: this.editSubject().trim(),
      bodyHtml: this.editBody(),
      isActive: this.editIsActive()
    }).subscribe({
      next: (updated) => {
        this.saving.set(false);
        this.templates.update((list) => list.map((t) => (t.templateCode === updated.templateCode ? updated : t)));
        this.selected.set(updated);
      },
      error: (err) => { this.saving.set(false); this.saveError.set(err?.error?.error ?? 'Unable to save template.'); }
    });
  }
}
