export interface LinkingLine {
  linkingLineId: number;
  equipmentId: number;
  equipmentName: string | null;
  linkedEquipmentId: number;
  linkedEquipmentName: string | null;
  remark: string | null;
  isActive: boolean;
}

export interface CreateLinkingLineRequest {
  equipmentId: number;
  linkedEquipmentId: number;
  remark: string | null;
}
