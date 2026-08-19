import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService } from '../../core/services/report.service';
import { ReportFilter, SavedReportFilter } from '../../core/models/report.model';
import { OutageListItem } from '../../core/models/outage.model';

const EMPTY_FILTER: ReportFilter = {
  zoneId: null,
  stationId: null,
  jobTypeId: null,
  outageCode: null,
  requestorStatus: null,
  gnmStatus: null,
  keyword: null,
  dateStart: null,
  dateEnd: null,
  showDraft: false,
  sortBy: 'date'
};

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css'
})
export class ReportsComponent {
  private reportService = inject(ReportService);

  filter = signal<ReportFilter>({ ...EMPTY_FILTER });
  results = signal<OutageListItem[]>([]);
  favourites = signal<SavedReportFilter[]>([]);
  loading = signal(false);
  exporting = signal<'excel' | 'pdf' | null>(null);
  errorMessage = signal<string | null>(null);
  hasGenerated = signal(false);

  showSaveForm = signal(false);
  favouriteName = signal('');
  savingFavourite = signal(false);

  constructor() {
    this.loadFavourites();
  }

  updateField<K extends keyof ReportFilter>(key: K, value: ReportFilter[K]): void {
    this.filter.update((f) => ({ ...f, [key]: value }));
  }

  onKeywordChange(value: string): void { this.updateField('keyword', value || null); }
  onDateStartChange(value: string): void { this.updateField('dateStart', value || null); }
  onDateEndChange(value: string): void { this.updateField('dateEnd', value || null); }
  onOutageCodeChange(value: string): void { this.updateField('outageCode', value || null); }
  onRequestorStatusChange(value: string): void { this.updateField('requestorStatus', value || null); }
  onGnmStatusChange(value: string): void { this.updateField('gnmStatus', value || null); }
  onSortByChange(value: string): void { this.updateField('sortBy', (value || 'date') as 'date' | 'code'); }
  onShowDraftChange(value: boolean): void { this.updateField('showDraft', value); }

  generate(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.reportService.generate(this.filter()).subscribe({
      next: (rows) => {
        this.results.set(rows);
        this.hasGenerated.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to generate report.');
        this.loading.set(false);
      }
    });
  }

  resetFilters(): void {
    this.filter.set({ ...EMPTY_FILTER });
    this.results.set([]);
    this.hasGenerated.set(false);
  }

  exportExcel(): void {
    this.exporting.set('excel');
    this.reportService.exportExcel(this.filter()).subscribe({
      next: (blob) => {
        this.reportService.triggerDownload(blob, `outage-report-${Date.now()}.xlsx`);
        this.exporting.set(null);
      },
      error: () => {
        this.errorMessage.set('Excel export failed.');
        this.exporting.set(null);
      }
    });
  }

  exportPdf(): void {
    this.exporting.set('pdf');
    this.reportService.exportPdf(this.filter()).subscribe({
      next: (blob) => {
        this.reportService.triggerDownload(blob, `outage-report-${Date.now()}.pdf`);
        this.exporting.set(null);
      },
      error: () => {
        this.errorMessage.set('PDF export failed.');
        this.exporting.set(null);
      }
    });
  }

  private loadFavourites(): void {
    this.reportService.listFavourites().subscribe({
      next: (favs) => this.favourites.set(favs),
      error: () => {}
    });
  }

  openSaveForm(): void {
    this.favouriteName.set('');
    this.showSaveForm.set(true);
  }

  cancelSaveForm(): void {
    this.showSaveForm.set(false);
  }

  saveFavourite(): void {
    if (!this.favouriteName().trim()) return;
    this.savingFavourite.set(true);
    this.reportService.saveFavourite({ filterName: this.favouriteName().trim(), filter: this.filter() }).subscribe({
      next: () => {
        this.savingFavourite.set(false);
        this.showSaveForm.set(false);
        this.loadFavourites();
      },
      error: (err) => {
        this.savingFavourite.set(false);
        this.errorMessage.set(err?.error?.error ?? 'Unable to save favourite.');
      }
    });
  }

  applyFavourite(fav: SavedReportFilter): void {
    this.filter.set({ ...EMPTY_FILTER, ...fav.filter });
    this.generate();
  }

  deleteFavourite(fav: SavedReportFilter, event: Event): void {
    event.stopPropagation();
    if (!confirm(`Delete favourite "${fav.filterName}"?`)) return;
    this.reportService.deleteFavourite(fav.savedReportFilterId).subscribe({
      next: () => this.loadFavourites(),
      error: () => {}
    });
  }
}
