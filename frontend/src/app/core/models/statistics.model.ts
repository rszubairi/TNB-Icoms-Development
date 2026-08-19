export interface ApprovedOutagesByDepartment {
  department: string;
  count: number;
}

export interface DepartmentStatusSummary {
  department: string;
  totalOutages: number;
  takenCompleted: number;
  notTaken: number;
  takenActive: number;
}

export interface OutageTypeBreakdown {
  outageTypeCode: string;
  count: number;
}

export interface RoutineMaintenanceSummary {
  department: string;
  completed: number;
  pending: number;
}

export interface StatisticsDashboard {
  approvedOutagesByDepartment: ApprovedOutagesByDepartment[];
  statusSummaryByDepartment: DepartmentStatusSummary[];
  outageTypeBreakdown: OutageTypeBreakdown[];
  routineMaintenanceByDepartment: RoutineMaintenanceSummary[];
}
