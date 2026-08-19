export interface Project {
  projectId: number;
  tpCode: string;
  projectSuffix: string;
  projectName: string;
  zoneId: number | null;
  zoneName: string | null;
  startDate: string | null;
  endDate: string | null;
  isActive: boolean;
  openOutageCount: number;
}

export interface CreateProjectRequest {
  tpCode: string;
  projectSuffix: string;
  zoneId: number | null;
}
