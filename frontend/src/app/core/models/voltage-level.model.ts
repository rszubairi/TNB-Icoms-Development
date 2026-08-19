export interface VoltageLevel {
  voltageLevelId: number;
  levelName: string;
  sortOrder: number;
  isActive: boolean;
  equipmentTypeCount: number;
}

export interface CreateVoltageLevelRequest {
  levelName: string;
}

export interface UpdateVoltageLevelRequest {
  levelName: string;
  sortOrder: number;
  isActive: boolean;
}
