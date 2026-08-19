export interface CommissioningMemoListItem {
  commissioningMemoId: number;
  outageId: number;
  outageNumber: string | null;
  memoNo: string;
  memoType: string;
  status: string;
  commissioningResult: string | null;
  submittedByName: string;
  submittedAt: string;
}

export interface CommissioningMemoDetail extends CommissioningMemoListItem {
  switchingProgram: string;
  dataForm: string | null;
  iomEndorsed: boolean;
  mtepProtectionLetter: boolean;
  residentEngineerCertification: boolean;
  formG: boolean;
  formH: boolean;
  meteringEmailChain: boolean;
  scadaEmailChain: boolean;
  hgsoLetterForGenerationPmu: boolean;
  rejectionReason: string | null;
  engineerPicApprovedByName: string | null;
  engineerPicApprovedAt: string | null;
  seApprovedByName: string | null;
  seApprovedAt: string | null;
  dceApprovedByName: string | null;
  dceApprovedAt: string | null;
  ceGnmApprovedByName: string | null;
  ceGnmApprovedAt: string | null;
  finalApprovedByName: string | null;
  finalApprovedAt: string | null;
}

export interface CreateCommissioningMemoRequest {
  outageId: number;
  memoType: string;
  switchingProgram: string;
  dataForm: string | null;
  iomEndorsed: boolean;
  mtepProtectionLetter: boolean;
  residentEngineerCertification: boolean;
  formG: boolean;
  formH: boolean;
  meteringEmailChain: boolean;
  scadaEmailChain: boolean;
  hgsoLetterForGenerationPmu: boolean;
}

export interface MemoStageReviewRequest {
  approve: boolean;
  rejectionReason: string | null;
}

export interface SetCommissioningResultRequest {
  commissioningResult: string;
}
