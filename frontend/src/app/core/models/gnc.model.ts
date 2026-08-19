import { OutageListItem, OutagePic } from './outage.model';

export interface GncOutageListItem extends OutageListItem {
  authorisationNo: string | null;
  takenActiveAt: string | null;
  takenCompletedAt: string | null;
  extendedTo: string | null;
  authoriserName: string | null;
  timelineStatus: 'OnTime' | 'Extended' | 'Overdue';
}

export interface TakeActiveRequest {
  authorisationNo: string;
  personnelId: number;
  takenActiveAt: string;
  remark: string | null;
}

export interface CompleteAuthorisationRequest {
  takenCompletedAt: string;
  remark: string | null;
}

export interface ExtendAuthorisationRequest {
  extendedTo: string;
  remark: string | null;
}

export interface NotTakenRequest {
  notTakenReasonId: number;
  remark: string | null;
}

export interface CancelOutageRequest {
  remark: string | null;
}

export interface ForcedOutageRequest {
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
  jobTypeId: number;
  description: string;
  approvedById: number;
  pics: OutagePic[];
}
