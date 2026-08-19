export interface Equipment {
  equipmentId: number;
  equipmentName: string;
  equipmentCode: string;
  shortName: string;
  zoneId: number;
  zoneName: string | null;
  stationId: number;
  stationName: string | null;
  voltageLevelId: number;
  voltageLevelName: string | null;
  equipmentTypeId: number;
  equipmentTypeName: string | null;
  mvaRatingId: number | null;
  mvaRatingLabel: string | null;
  position: number; // 0 = Closed, 1 = Open
  isOffPoint: boolean;
  offPointRemark: string | null;
  isActive: boolean;
}

export interface CreateEquipmentRequest {
  stationId: number;
  voltageLevelId: number;
  equipmentTypeId: number;
  name: string;
  mvaRatingId: number | null;
  isOpen: boolean;
  isOffPoint: boolean;
  offPointRemark: string | null;
}

export interface UpdateEquipmentRequest {
  name: string;
  mvaRatingId: number | null;
  isOpen: boolean;
  isOffPoint: boolean;
  offPointRemark: string | null;
  isActive: boolean;
}
