export interface GeneratedName {
  stationId: number;
  stationAbbr: string;
  generatedName: string;
}

export interface TransmissionLine {
  transmissionLineId: number;
  voltageLevelId: number;
  voltageLevelName: string | null;
  equipmentTypeId: number;
  equipmentTypeName: string | null;
  namingInteger: number;
  lineNumber: number;
  lineFilterType: string;
  isActive: boolean;
  generatedNames: GeneratedName[];
  ownerZoneIds: number[];
  ownerZoneNames: string[];
}

export interface TransmissionLineRequest {
  voltageLevelId: number;
  equipmentTypeId: number;
  namingInteger: number;
  lineNumber: number;
  stationIdsInOrder: number[];
}
