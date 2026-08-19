export interface SldListItem {
  singleLineDiagramId: number;
  stationId: number;
  stationName: string | null;
  voltageLevelName: string | null;
  flowType: string;
  mnemonic: string | null;
  substationType: string | null;
  diagramNumber: string | null;
  status: string;
  hasDrawing: boolean;
  submittedByName: string;
  submittedAt: string;
}

export interface SldDetail extends SldListItem {
  voltageLevelId: number;
  rejectionReason: string | null;
  remark: string | null;
  engineerReviewedByName: string | null;
  engineerReviewedAt: string | null;
  seApprovedByName: string | null;
  seApprovedAt: string | null;
  dceApprovedByName: string | null;
  dceApprovedAt: string | null;
  requestorApprovedByName: string | null;
  requestorApprovedAt: string | null;
  publishedAt: string | null;
}

export interface CreateSldRequest {
  stationId: number;
  voltageLevelId: number;
  flowType: string;
  remark: string | null;
}

export interface EngineerReviewRequest {
  approve: boolean;
  mnemonic: string | null;
  substationType: string | null;
  rejectionReason: string | null;
}

export interface StageReviewRequest {
  approve: boolean;
  rejectionReason: string | null;
}
