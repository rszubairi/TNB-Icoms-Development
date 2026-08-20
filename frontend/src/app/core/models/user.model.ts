export interface UserListItem {
  tnbId: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  roleId: string;
  roleName: string;
  zoneId: string;
  zoneName: string;
  isActive: boolean;
}

export interface UserDetail extends UserListItem {
  organisationId: string | null;
  gcuTypeId: string | null;
  gcuStationIds: string[];
}

export interface CreateUserRequest {
  tnbId: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  roleId: string;
  zoneId: string;
  organisationId: string | null;
  gcuTypeId: string | null;
  gcuStationIds: string[];
  isActive: boolean;
}

export interface UpdateUserRequest extends CreateUserRequest {}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
