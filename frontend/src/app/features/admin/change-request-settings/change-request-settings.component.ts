import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChangeRequestSettingsService } from '../../../core/services/change-request-settings.service';

@Component({
  selector: 'app-change-request-settings',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './change-request-settings.component.html',
  styleUrl: './change-request-settings.component.css'
})
export class ChangeRequestSettingsComponent {
  private settingsService = inject(ChangeRequestSettingsService);

  days = signal<number>(7);
  loading = signal(true);
  saving = signal(false);
  errorMessage = signal<string | null>(null);
  saveMessage = signal<string | null>(null);

  constructor() {
    this.settingsService.get().subscribe({
      next: (setting) => {
        this.days.set(Number(setting.settingValue));
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load setting. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  updateDays(value: number): void {
    this.days.set(value);
    this.saveMessage.set(null);
  }

  save(): void {
    if (!this.days() || this.days() < 1) {
      this.errorMessage.set('Enter a whole number of days, 1 or greater.');
      return;
    }

    this.errorMessage.set(null);
    this.saving.set(true);
    this.settingsService.save(this.days()).subscribe({
      next: () => {
        this.saving.set(false);
        this.saveMessage.set('Changes apply immediately to all active users.');
      },
      error: (err) => {
        this.saving.set(false);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to save setting.');
      }
    });
  }
}
