import { OutageListItem } from './outage.model';

export interface WeeklyOutageCount {
  year: number;
  weekNumber: number;
  weekLabel: string;
  weekStart: string;
  count: number;
}

export interface StatusBreakdown {
  status: string;
  count: number;
}

export interface DashboardMetrics {
  totalOutages: number;
  pendingPlannerReview: number;
  pendingGnmApproval: number;
  activeNow: number;
  emergencyOpen: number;
  closedThisMonth: number;
}

export interface Dashboard {
  metrics: DashboardMetrics;
  weeklyOutageCounts: WeeklyOutageCount[];
  statusBreakdown: StatusBreakdown[];
  inProgress: OutageListItem[];
  emergencyRequests: OutageListItem[];
}
