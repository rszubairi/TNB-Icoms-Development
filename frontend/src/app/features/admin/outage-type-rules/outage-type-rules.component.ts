import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OutageTypeRuleService } from '../../../core/services/outage-type-rule.service';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { OutageTypeRule } from '../../../core/models/outage-type-rule.model';
import { VoltageLevel } from '../../../core/models/voltage-level.model';

interface RuleDraft {
  outageTypeRuleId: number | null;
  outageTypeCode: string;
  workTypeCode: string;
  moreThanDays: number | null;
  moreThanMonths: number | null;
  moreThanYears: number | null;
  lessThanDays: number | null;
  lessThanMonths: number | null;
  lessThanYears: number | null;
  voltages: string[]; // empty = ALL
}

const emptyDraft: RuleDraft = {
  outageTypeRuleId: null,
  outageTypeCode: 'Planned',
  workTypeCode: 'Dead',
  moreThanDays: null,
  moreThanMonths: null,
  moreThanYears: null,
  lessThanDays: null,
  lessThanMonths: null,
  lessThanYears: null,
  voltages: []
};

export const OUTAGE_TYPES = ['Planned', 'Unplanned', 'Emergency', 'Forced'];
export const WORK_TYPES = ['Dead', 'Live'];

@Component({
  selector: 'app-outage-type-rules',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './outage-type-rules.component.html',
  styleUrl: './outage-type-rules.component.css'
})
export class OutageTypeRulesComponent {
  private ruleService = inject(OutageTypeRuleService);
  private voltageLevelService = inject(VoltageLevelService);

  outageTypes = OUTAGE_TYPES;
  workTypes = WORK_TYPES;

  rules = signal<OutageTypeRule[]>([]);
  voltageLevels = signal<VoltageLevel[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  showForm = signal(false);
  draft = signal<RuleDraft>({ ...emptyDraft });
  formError = signal<string | null>(null);
  saving = signal(false);
  rowBusyId = signal<number | null>(null);

  isEditMode = computed(() => this.draft().outageTypeRuleId != null);

  deadRules = computed(() => this.rules().filter((r) => r.workTypeCode === 'Dead' && r.isActive));
  liveRules = computed(() => this.rules().filter((r) => r.workTypeCode === 'Live' && r.isActive));

  constructor() {
    this.voltageLevelService.list().subscribe({ next: (levels) => this.voltageLevels.set(levels), error: () => {} });
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.ruleService.list().subscribe({
      next: (rules) => {
        this.rules.set(rules);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load outage type rules. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  formatRange(rule: OutageTypeRule): string {
    const more = this.formatDuration(rule.moreThanDays, rule.moreThanMonths, rule.moreThanYears);
    const less = this.formatDuration(rule.lessThanDays, rule.lessThanMonths, rule.lessThanYears);
    if (more && less) return `More than ${more}, less than ${less}`;
    if (more) return `More than ${more}`;
    if (less) return `Less than ${less}`;
    return '—';
  }

  private formatDuration(days: number | null, months: number | null, years: number | null): string {
    const parts: string[] = [];
    if (years) parts.push(`${years}y`);
    if (months) parts.push(`${months}mo`);
    if (days) parts.push(`${days}d`);
    return parts.join(' ');
  }

  startCreate(): void {
    this.formError.set(null);
    this.draft.set({ ...emptyDraft });
    this.showForm.set(true);
  }

  startEdit(rule: OutageTypeRule): void {
    this.formError.set(null);
    this.draft.set({
      outageTypeRuleId: rule.outageTypeRuleId,
      outageTypeCode: rule.outageTypeCode,
      workTypeCode: rule.workTypeCode,
      moreThanDays: rule.moreThanDays,
      moreThanMonths: rule.moreThanMonths,
      moreThanYears: rule.moreThanYears,
      lessThanDays: rule.lessThanDays,
      lessThanMonths: rule.lessThanMonths,
      lessThanYears: rule.lessThanYears,
      voltages: rule.appliesTo.trim().toUpperCase() === 'ALL' ? [] : rule.appliesTo.split(',').map((v) => v.trim())
    });
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.formError.set(null);
  }

  updateField<K extends keyof RuleDraft>(field: K, value: RuleDraft[K]): void {
    this.draft.update((d) => ({ ...d, [field]: value }));
  }

  toggleVoltage(name: string, checked: boolean): void {
    this.draft.update((d) => {
      const set = new Set(d.voltages);
      if (checked) set.add(name);
      else set.delete(name);
      return { ...d, voltages: Array.from(set) };
    });
  }

  isVoltageChecked(name: string): boolean {
    return this.draft().voltages.includes(name);
  }

  save(): void {
    const draft = this.draft();
    this.formError.set(null);
    this.saving.set(true);

    const request = {
      outageTypeCode: draft.outageTypeCode,
      workTypeCode: draft.workTypeCode,
      moreThanDays: draft.moreThanDays,
      moreThanMonths: draft.moreThanMonths,
      moreThanYears: draft.moreThanYears,
      lessThanDays: draft.lessThanDays,
      lessThanMonths: draft.lessThanMonths,
      lessThanYears: draft.lessThanYears,
      appliesTo: draft.voltages.length === 0 ? 'ALL' : draft.voltages.join(',')
    };

    const request$ = draft.outageTypeRuleId
      ? this.ruleService.update(draft.outageTypeRuleId, request)
      : this.ruleService.create(request);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.cancelForm();
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to save rule.');
      }
    });
  }

  deactivate(rule: OutageTypeRule): void {
    if (!confirm(`Remove this ${rule.outageTypeCode} rule for ${rule.workTypeCode} outages?`)) return;
    this.rowBusyId.set(rule.outageTypeRuleId);
    this.ruleService.deactivate(rule.outageTypeRuleId).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to remove rule.');
      }
    });
  }
}
