export interface EmailTemplate {
  emailTemplateId: number;
  templateCode: string;
  name: string;
  subject: string;
  bodyHtml: string;
  availableTags: string;
  isActive: boolean;
  updatedAt: string;
  updatedByName: string | null;
}

export interface UpdateEmailTemplateRequest {
  subject: string;
  bodyHtml: string;
  isActive: boolean;
}
