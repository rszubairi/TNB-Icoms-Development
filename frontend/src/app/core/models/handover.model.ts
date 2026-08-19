export interface HandoverEntry {
  handoverEntryId: number;
  category: string;
  description: string;
  relatedOutageId: number | null;
  relatedOutageNumber: string | null;
  isLocked: boolean;
  createdByName: string;
  createdAt: string;
}

export interface HandoverShift {
  shiftId: number;
  shiftDate: string;
  shiftType: 'Morning' | 'Evening' | 'Night';
  zoneId: number;
  zoneName: string | null;
  controlManagerId: number | null;
  controlManagerName: string | null;
  switchEngineer1Id: number | null;
  switchEngineer1Name: string | null;
  switchEngineer2Id: number | null;
  switchEngineer2Name: string | null;
  despatcherId: number | null;
  despatcherName: string | null;
  controlAssistantId: number | null;
  controlAssistantName: string | null;
  isPassed: boolean;
  passedAt: string | null;
  passedByName: string | null;
  entries: HandoverEntry[];
}

export interface HandoverShiftSummary {
  shiftId: number;
  shiftDate: string;
  shiftType: string;
  isPassed: boolean;
  entryCount: number;
}

export interface UpdateShiftControlRequest {
  controlManagerId: number | null;
  switchEngineer1Id: number | null;
  switchEngineer2Id: number | null;
  despatcherId: number | null;
  controlAssistantId: number | null;
}

export interface AddEntryRequest {
  category: string;
  description: string;
  relatedOutageId: number | null;
}
