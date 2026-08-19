import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MnemonicService } from '../../../core/services/mnemonic.service';
import { MnemonicDocument } from '../../../core/models/mnemonic-document.model';

@Component({
  selector: 'app-mnemonic',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './mnemonic.component.html',
  styleUrl: './mnemonic.component.css'
})
export class MnemonicComponent {
  private mnemonicService = inject(MnemonicService);

  documents = signal<MnemonicDocument[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);
  uploading = signal(false);
  downloadingId = signal<number | 'current' | null>(null);

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.mnemonicService.list().subscribe({
      next: (documents) => {
        this.documents.set(documents);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load the Mnemonic list history. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (!file.name.toLowerCase().endsWith('.pdf')) {
      this.errorMessage.set('Only PDF files are accepted.');
      input.value = '';
      return;
    }

    this.errorMessage.set(null);
    this.uploading.set(true);

    this.mnemonicService.upload(file).subscribe({
      next: () => {
        this.uploading.set(false);
        input.value = '';
        this.load();
      },
      error: (err) => {
        this.uploading.set(false);
        input.value = '';
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to upload file.');
      }
    });
  }

  downloadCurrent(): void {
    this.downloadingId.set('current');
    this.mnemonicService.downloadCurrent().subscribe({
      next: (blob) => {
        const current = this.documents().find((d) => d.isCurrent);
        this.mnemonicService.triggerDownload(blob, current?.originalFileName ?? 'Mnemonic.pdf');
        this.downloadingId.set(null);
      },
      error: () => {
        this.downloadingId.set(null);
        this.errorMessage.set('Unable to download the current Mnemonic list.');
      }
    });
  }

  download(doc: MnemonicDocument): void {
    this.downloadingId.set(doc.mnemonicDocumentId);
    this.mnemonicService.download(doc.mnemonicDocumentId).subscribe({
      next: (blob) => {
        this.mnemonicService.triggerDownload(blob, doc.originalFileName);
        this.downloadingId.set(null);
      },
      error: () => {
        this.downloadingId.set(null);
        this.errorMessage.set('Unable to download this file.');
      }
    });
  }

  formatSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }
}
