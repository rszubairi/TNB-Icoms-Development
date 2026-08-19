export interface ErrorLog {
  errorLogId: number;
  source: string;
  severity: string;
  message: string;
  stackTrace: string | null;
  url: string | null;
  userName: string | null;
  userAgent: string | null;
  occurredAt: string;
}
