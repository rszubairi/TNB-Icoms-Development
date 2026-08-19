export interface ReportFilter {
  zoneId?: number | null;
  stationId?: number | null;
  jobTypeId?: number | null;
  outageCode?: string | null;
  requestorStatus?: string | null;
  gnmStatus?: string | null;
  keyword?: string | null;
  dateStart?: string | null;
  dateEnd?: string | null;
  showDraft: boolean;
  sortBy?: 'date' | 'code' | null;
}

export interface SavedReportFilter {
  savedReportFilterId: number;
  filterName: string;
  filter: ReportFilter;
  createdAt: string;
}

export interface SaveReportFilterRequest {
  filterName: string;
  filter: ReportFilter;
}
