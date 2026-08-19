export interface EmailLog {
  emailLogId: number;
  templateCode: string | null;
  toAddress: string;
  subject: string;
  bodyHtml: string;
  status: string;
  errorMessage: string | null;
  sentAt: string;
}
