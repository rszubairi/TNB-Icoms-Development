export interface OutageTypeRule {
  outageTypeRuleId: number;
  outageTypeCode: string;
  workTypeCode: string;
  moreThanDays: number | null;
  moreThanMonths: number | null;
  moreThanYears: number | null;
  lessThanDays: number | null;
  lessThanMonths: number | null;
  lessThanYears: number | null;
  appliesTo: string;
  isActive: boolean;
}

export interface SaveOutageTypeRuleRequest {
  outageTypeCode: string;
  workTypeCode: string;
  moreThanDays: number | null;
  moreThanMonths: number | null;
  moreThanYears: number | null;
  lessThanDays: number | null;
  lessThanMonths: number | null;
  lessThanYears: number | null;
  appliesTo: string;
}
