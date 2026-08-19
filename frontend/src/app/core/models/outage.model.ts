export interface OutagePic {
  picName: string;
  picEmail: string;
  picPhone: string | null;
}

export interface OutageListItem {
  outageId: number;
  outageNumber: string;
  outageCode: string;
  outageTypeCode: string;
  outageClass: string;
  workTypeCode: string;
  zoneName: string | null;
  stationName: string | null;
  voltageLevelName: string | null;
  equipmentName: string | null;
  jobTypeLabel: string | null;
  projectName: string | null;
  plannedStartAt: string;
  plannedEndAt: string;
  restorationLabel: string | null;
  requestorStatus: string;
  plannerStatus: string | null;
  gnmStatus: string;
  gncStatus: string | null;
  description: string;
  picNames: string[];
  justification: string | null;
  highlights: string | null;
  remark: string | null;
  underStudyNotes: string | null;
  createdByName: string;
  createdAt: string;
}

export interface ConflictingOutage {
  outageId: number;
  outageNumber: string;
  equipmentName: string | null;
  plannedStartAt: string;
  plannedEndAt: string;
}

export interface OutageDetail extends OutageListItem {
  zoneId: number;
  stationId: number;
  voltageLevelId: number;
  primaryEquipmentId: number;
  equipmentTypeId: number;
  additionalEquipmentIds: number[];
  sequenceId: number | null;
  restorationId: number | null;
  jobTypeId: number;
  projectId: number | null;
  hasPtw: boolean;
  pics: OutagePic[];
  conflictingOutages: ConflictingOutage[];
  warnings: string[];
}

export interface CreateOutageRequest {
  stationId: number;
  voltageLevelId: number;
  equipmentTypeId: number;
  primaryEquipmentId: number;
  lineFilterType: string | null;
  additionalEquipmentIds: number[];
  workTypeCode: string;
  hasPtw: boolean;
  plannedStartAt: string;
  plannedEndAt: string;
  sequenceId: number | null;
  restorationId: number | null;
  jobTypeId: number;
  projectId: number | null;
  description: string;
  pics: OutagePic[];
  submitNow: boolean;
}

export interface OutageListFilter {
  zoneId?: number;
  requestorStatus?: string;
  gnmStatus?: string;
  outageCode?: string;
  pendingPlannerReviewOnly?: boolean;
  pendingConfirmationOnly?: boolean;
  agreedAndConfirmedOnly?: boolean;
  pendingGnmApprovalOnly?: boolean;
  rangeStart?: string;
  rangeEnd?: string;
}

export interface BulkActionResult {
  succeededCount: number;
  errors: string[];
}
