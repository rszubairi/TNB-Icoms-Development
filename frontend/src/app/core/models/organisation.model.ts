export interface Organisation {
  organisationId: number;
  organisationName: string;
  organisationCode: string;
  zoneId: number;
  zoneName: string | null;
  isGcu: boolean;
  isActive: boolean;
}

export interface CreateOrganisationRequest {
  organisationName: string;
  organisationCode: string;
  zoneId: number;
  isGcu: boolean;
}

export interface UpdateOrganisationRequest {
  organisationName: string;
  organisationCode: string;
  isGcu: boolean;
  isActive: boolean;
}
