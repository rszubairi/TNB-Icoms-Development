import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DropdownValueService } from '../../../core/services/dropdown-value.service';
import { DropdownCategory, DropdownValue } from '../../../core/models/dropdown-value.model';

interface ValueDraft {
  dropdownValueId: number | null;
  valueLabel: string;
  parentCode: string | null;
  isActive: boolean;
}

const emptyDraft: ValueDraft = { dropdownValueId: null, valueLabel: '', parentCode: null, isActive: true };

@Component({
  selector: 'app-dropdown-values',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './dropdown-values.component.html',
  styleUrl: './dropdown-values.component.css'
})
export class DropdownValuesComponent {
  private dropdownValueService = inject(DropdownValueService);

  categories = signal<DropdownCategory[]>([]);
  outageTypeParents = signal<string[]>([]);
  selectedCategoryCode = signal<string | null>(null);
  values = signal<DropdownValue[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);

  draft = signal<ValueDraft>({ ...emptyDraft });
  formError = signal<string | null>(null);
  saving = signal(false);
  rowBusyId = signal<number | null>(null);
  isAdding = signal(false);

  isEditMode = computed(() => this.draft().dropdownValueId != null);
  showForm = computed(() => this.isEditMode() || this.isAdding());
  selectedCategory = computed(() => this.categories().find((c) => c.code === this.selectedCategoryCode()) ?? null);

  constructor() {
    this.dropdownValueService.listCategories().subscribe({
      next: (res) => {
        this.categories.set(res.categories);
        this.outageTypeParents.set(res.outageTypeParents);
        if (res.categories.length > 0) {
          this.selectedCategoryCode.set(res.categories[0].code);
          this.loadValues();
        } else {
          this.loading.set(false);
        }
      },
      error: () => {
        this.errorMessage.set('Unable to load dropdown categories. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  private loadValues(): void {
    const category = this.selectedCategoryCode();
    if (!category) return;

    this.loading.set(true);
    this.errorMessage.set(null);
    this.dropdownValueService.listForAdmin(category).subscribe({
      next: (values) => {
        this.values.set(values);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load dropdown values.');
        this.loading.set(false);
      }
    });
  }

  selectCategory(code: string): void {
    this.selectedCategoryCode.set(code);
    this.resetForm();
    this.loadValues();
  }

  startCreate(): void {
    this.formError.set(null);
    this.draft.set({ ...emptyDraft });
    this.isAdding.set(true);
  }

  startEdit(value: DropdownValue): void {
    this.formError.set(null);
    this.draft.set({
      dropdownValueId: value.dropdownValueId,
      valueLabel: value.valueLabel,
      parentCode: value.parentCode,
      isActive: value.isActive
    });
  }

  resetForm(): void {
    this.draft.set({ ...emptyDraft });
    this.formError.set(null);
    this.isAdding.set(false);
  }

  updateLabel(value: string): void {
    this.draft.update((d) => ({ ...d, valueLabel: value }));
  }

  updateParentCode(value: string): void {
    this.draft.update((d) => ({ ...d, parentCode: value || null }));
  }

  updateIsActive(value: boolean): void {
    this.draft.update((d) => ({ ...d, isActive: value }));
  }

  save(): void {
    const draft = this.draft();
    const category = this.selectedCategoryCode();
    if (!category) return;

    if (!draft.valueLabel.trim()) {
      this.formError.set('Value is required.');
      return;
    }

    this.formError.set(null);
    this.saving.set(true);

    const request$ = draft.dropdownValueId
      ? this.dropdownValueService.update(draft.dropdownValueId, {
          valueLabel: draft.valueLabel.trim(),
          parentCode: draft.parentCode,
          isActive: draft.isActive
        })
      : this.dropdownValueService.create({
          categoryCode: category,
          valueLabel: draft.valueLabel.trim(),
          parentCode: draft.parentCode
        });

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.resetForm();
        this.loadValues();
      },
      error: (err) => {
        this.saving.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to save value.');
      }
    });
  }

  move(value: DropdownValue, direction: 'up' | 'down'): void {
    this.rowBusyId.set(value.dropdownValueId);
    this.dropdownValueService.reorder(value.dropdownValueId, direction).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.loadValues();
      },
      error: () => {
        this.rowBusyId.set(null);
      }
    });
  }

  deactivate(value: DropdownValue): void {
    if (!confirm(`Deactivate "${value.valueLabel}"?`)) return;
    this.rowBusyId.set(value.dropdownValueId);
    this.dropdownValueService.deactivate(value.dropdownValueId).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.loadValues();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate value.');
      }
    });
  }
}
