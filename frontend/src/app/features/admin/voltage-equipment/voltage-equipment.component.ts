import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { VoltageLevelService } from '../../../core/services/voltage-level.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { VoltageLevel } from '../../../core/models/voltage-level.model';
import { EquipmentType } from '../../../core/models/equipment-type.model';

interface VoltageDraft {
  voltageLevelId: number | null;
  levelName: string;
  sortOrder: number;
  isActive: boolean;
}

interface TypeDraft {
  equipmentTypeId: number | null;
  typeName: string;
  isActive: boolean;
}

const emptyVoltageDraft: VoltageDraft = { voltageLevelId: null, levelName: '', sortOrder: 0, isActive: true };
const emptyTypeDraft: TypeDraft = { equipmentTypeId: null, typeName: '', isActive: true };

@Component({
  selector: 'app-voltage-equipment',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './voltage-equipment.component.html',
  styleUrl: './voltage-equipment.component.css'
})
export class VoltageEquipmentComponent {
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentTypeService = inject(EquipmentTypeService);

  voltageLevels = signal<VoltageLevel[]>([]);
  selectedVoltageLevelId = signal<number | null>(null);
  equipmentTypes = signal<EquipmentType[]>([]);

  loading = signal(true);
  errorMessage = signal<string | null>(null);

  voltageDraft = signal<VoltageDraft>({ ...emptyVoltageDraft });
  voltageFormError = signal<string | null>(null);
  voltageSaving = signal(false);
  voltageBusyId = signal<number | null>(null);
  isAddingVoltage = signal(false);

  typeDraft = signal<TypeDraft>({ ...emptyTypeDraft });
  typeFormError = signal<string | null>(null);
  typeSaving = signal(false);
  typeBusyId = signal<number | null>(null);
  isAddingType = signal(false);

  isVoltageEditMode = computed(() => this.voltageDraft().voltageLevelId != null);
  isTypeEditMode = computed(() => this.typeDraft().equipmentTypeId != null);
  showVoltageForm = computed(() => this.isVoltageEditMode() || this.isAddingVoltage());
  showTypeForm = computed(() => this.isTypeEditMode() || this.isAddingType());

  selectedVoltage = computed(() => this.voltageLevels().find((v) => v.voltageLevelId === this.selectedVoltageLevelId()) ?? null);

  constructor() {
    this.loadVoltageLevels();
  }

  private loadVoltageLevels(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.voltageLevelService.list().subscribe({
      next: (levels) => {
        this.voltageLevels.set(levels);
        if (levels.length > 0 && this.selectedVoltageLevelId() == null) {
          this.selectedVoltageLevelId.set(levels[0].voltageLevelId);
        }
        this.loading.set(false);
        this.loadEquipmentTypes();
      },
      error: () => {
        this.errorMessage.set('Unable to load voltage levels. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  private loadEquipmentTypes(): void {
    const voltageLevelId = this.selectedVoltageLevelId();
    if (!voltageLevelId) {
      this.equipmentTypes.set([]);
      return;
    }
    this.equipmentTypeService.list(voltageLevelId).subscribe({
      next: (types) => this.equipmentTypes.set(types),
      error: () => this.errorMessage.set('Unable to load equipment types.')
    });
  }

  selectVoltage(voltageLevelId: number): void {
    this.selectedVoltageLevelId.set(voltageLevelId);
    this.resetTypeForm();
    this.loadEquipmentTypes();
  }

  // --- Voltage Levels ---

  startCreateVoltage(): void {
    this.voltageFormError.set(null);
    this.voltageDraft.set({ ...emptyVoltageDraft });
    this.isAddingVoltage.set(true);
  }

  startEditVoltage(level: VoltageLevel): void {
    this.voltageFormError.set(null);
    this.voltageDraft.set({
      voltageLevelId: level.voltageLevelId,
      levelName: level.levelName,
      sortOrder: level.sortOrder,
      isActive: level.isActive
    });
  }

  resetVoltageForm(): void {
    this.voltageDraft.set({ ...emptyVoltageDraft });
    this.voltageFormError.set(null);
    this.isAddingVoltage.set(false);
  }

  updateVoltageName(value: string): void {
    this.voltageDraft.update((d) => ({ ...d, levelName: value }));
  }

  saveVoltage(): void {
    const draft = this.voltageDraft();
    if (!draft.levelName.trim()) {
      this.voltageFormError.set('Voltage level name is required.');
      return;
    }

    this.voltageFormError.set(null);
    this.voltageSaving.set(true);

    const request$ = draft.voltageLevelId
      ? this.voltageLevelService.update(draft.voltageLevelId, {
          levelName: draft.levelName.trim(),
          sortOrder: draft.sortOrder,
          isActive: draft.isActive
        })
      : this.voltageLevelService.create({ levelName: draft.levelName.trim() });

    request$.subscribe({
      next: (saved) => {
        this.voltageSaving.set(false);
        this.resetVoltageForm();
        this.selectedVoltageLevelId.set(saved.voltageLevelId);
        this.loadVoltageLevels();
      },
      error: (err) => {
        this.voltageSaving.set(false);
        this.voltageFormError.set(err?.error?.error ?? err?.message ?? 'Unable to save voltage level.');
      }
    });
  }

  deactivateVoltage(level: VoltageLevel): void {
    if (!confirm(`Deactivate "${level.levelName}"? It must have no active equipment types first.`)) return;
    this.voltageBusyId.set(level.voltageLevelId);
    this.voltageLevelService.deactivate(level.voltageLevelId).subscribe({
      next: () => {
        this.voltageBusyId.set(null);
        this.loadVoltageLevels();
      },
      error: (err) => {
        this.voltageBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate voltage level.');
      }
    });
  }

  // --- Equipment Types ---

  startCreateType(): void {
    this.typeFormError.set(null);
    this.typeDraft.set({ ...emptyTypeDraft });
    this.isAddingType.set(true);
  }

  startEditType(type: EquipmentType): void {
    this.typeFormError.set(null);
    this.typeDraft.set({
      equipmentTypeId: type.equipmentTypeId,
      typeName: type.typeName,
      isActive: type.isActive
    });
  }

  resetTypeForm(): void {
    this.typeDraft.set({ ...emptyTypeDraft });
    this.typeFormError.set(null);
    this.isAddingType.set(false);
  }

  updateTypeName(value: string): void {
    this.typeDraft.update((d) => ({ ...d, typeName: value }));
  }

  updateTypeIsActive(value: boolean): void {
    this.typeDraft.update((d) => ({ ...d, isActive: value }));
  }

  saveType(): void {
    const draft = this.typeDraft();
    const voltageLevelId = this.selectedVoltageLevelId();
    if (!voltageLevelId) return;

    if (!draft.typeName.trim()) {
      this.typeFormError.set('Equipment type name is required.');
      return;
    }

    this.typeFormError.set(null);
    this.typeSaving.set(true);

    const request$ = draft.equipmentTypeId
      ? this.equipmentTypeService.update(draft.equipmentTypeId, {
          typeName: draft.typeName.trim(),
          isActive: draft.isActive
        })
      : this.equipmentTypeService.create({ typeName: draft.typeName.trim(), voltageLevelId });

    request$.subscribe({
      next: () => {
        this.typeSaving.set(false);
        this.resetTypeForm();
        this.loadEquipmentTypes();
      },
      error: (err) => {
        this.typeSaving.set(false);
        this.typeFormError.set(err?.error?.error ?? err?.message ?? 'Unable to save equipment type.');
      }
    });
  }

  deactivateType(type: EquipmentType): void {
    if (!confirm(`Deactivate "${type.typeName}"? It must have no active equipment first.`)) return;
    this.typeBusyId.set(type.equipmentTypeId);
    this.equipmentTypeService.deactivate(type.equipmentTypeId).subscribe({
      next: () => {
        this.typeBusyId.set(null);
        this.loadEquipmentTypes();
      },
      error: (err) => {
        this.typeBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate equipment type.');
      }
    });
  }
}
