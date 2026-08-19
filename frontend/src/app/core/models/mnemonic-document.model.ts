export interface MnemonicDocument {
  mnemonicDocumentId: number;
  originalFileName: string;
  fileSizeBytes: number;
  uploadedByName: string | null;
  uploadedAt: string;
  isCurrent: boolean;
}
