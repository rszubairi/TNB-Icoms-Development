export interface EquipmentType {
  equipmentTypeId: number;
  typeName: string;
  typeCode: string | null;
  voltageLevelId: number;
  voltageLevelName: string | null;
  isActive: boolean;
}

export interface CreateEquipmentTypeRequest {
  typeName: string;
  voltageLevelId: number;
}

export interface UpdateEquipmentTypeRequest {
  typeName: string;
  isActive: boolean;
}
