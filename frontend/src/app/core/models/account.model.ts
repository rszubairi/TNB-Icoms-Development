export interface AccountProfile {
  userId: number;
  fullName: string;
  email: string;
  phoneNumber: string | null;
  roleName: string | null;
  zoneName: string | null;
  isExternal: boolean;
}

export interface UpdateAccountProfileRequest {
  fullName: string;
  email: string;
  phoneNumber: string | null;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
