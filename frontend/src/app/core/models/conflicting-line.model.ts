export interface ConflictingLine {
  conflictingLineId: number;
  equipmentId: number;
  equipmentName: string | null;
  conflictingEquipmentId: number;
  conflictingEquipmentName: string | null;
  remark: string | null;
  isActive: boolean;
}

export interface CreateConflictingLineRequest {
  equipmentId: number;
  conflictingEquipmentId: number;
  remark: string | null;
}
