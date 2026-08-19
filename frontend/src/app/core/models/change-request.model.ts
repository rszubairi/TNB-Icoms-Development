export interface ChangeRequestField {
  fieldName: string;
  oldValue: string | null;
  newValue: string | null;
}

export interface ChangeRequestBatch {
  batchId: string;
  outageId: number;
  outageNumber: string | null;
  status: string;
  reason: string | null;
  requestedByName: string | null;
  requestedAt: string;
  reviewedByName: string | null;
  reviewedAt: string | null;
  reviewComment: string | null;
  fields: ChangeRequestField[];
}

export interface CreateChangeRequestBatch {
  outageId: number;
  reason: string;
  newPlannedStartAt: string | null;
  newPlannedEndAt: string | null;
  newVoltageLevelId: number | null;
  newPrimaryEquipmentId: number | null;
  addAdditionalEquipmentIds: number[];
}
