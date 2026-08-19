export interface Station {
  stationId: number;
  stationName: string;
  stationAbbr: string;
  zoneId: number;
  zoneName: string | null;
  orgId: number;
  organisationName: string | null;
  sldFileUrl: string | null;
  isActive: boolean;
}

export interface CreateStationRequest {
  stationName: string;
  stationAbbr: string;
  zoneId: number;
  orgId: number;
}

export interface UpdateStationRequest {
  stationName: string;
  stationAbbr: string;
  orgId: number;
  isActive: boolean;
}
