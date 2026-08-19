export interface LoginRequest {
  identifier: string;
  password: string;
  rememberMe: boolean;
}

export interface AuthenticatedUser {
  tnbId: string;
  fullName: string;
  email: string;
  roleId: string;
  roleName: string;
  zoneId: string;
  zoneName: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: AuthenticatedUser;
}
