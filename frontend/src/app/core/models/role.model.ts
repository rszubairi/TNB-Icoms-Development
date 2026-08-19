export interface RolePermission {
  moduleCode: string;
  permissionCode: string;
  isGranted: boolean;
}

export interface Role {
  roleId: number;
  roleName: string;
  roleCode: string;
  isExternal: boolean;
  isActive: boolean;
  permissionCount: number;
}

export interface RoleDetail {
  roleId: number;
  roleName: string;
  roleCode: string;
  isExternal: boolean;
  isActive: boolean;
  createdAt: string;
  permissions: RolePermission[];
}

export interface CreateRoleRequest {
  roleName: string;
  roleCode: string;
  isExternal: boolean;
  permissions: RolePermission[];
}

export interface UpdateRoleRequest {
  roleName: string;
  isExternal: boolean;
  isActive: boolean;
  permissions: RolePermission[];
}

export interface PermissionModule {
  code: string;
  label: string;
}

export interface PermissionModulesResponse {
  modules: PermissionModule[];
  actions: string[];
}
