export interface AuthorisationPersonnel {
  authorisationPersonnelId: number;
  fullName: string;
  email: string;
  staffId: string | null;
  zoneId: number;
  zoneName: string | null;
  designation: string | null;
  isActive: boolean;
}

export interface SaveAuthorisationPersonnelRequest {
  fullName: string;
  email: string;
  staffId: string | null;
  zoneId: number;
  designation: string | null;
  isActive: boolean;
}
