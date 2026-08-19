import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RoleService } from '../../../../core/services/role.service';
import { PermissionModule, Role, RoleDetail, RolePermission } from '../../../../core/models/role.model';

interface EditableRole {
  roleId: number | null;
  roleName: string;
  roleCode: string;
  isExternal: boolean;
  isActive: boolean;
  grants: Record<string, boolean>;
}

function gridKey(moduleCode: string, action: string): string {
  return `${moduleCode}::${action}`;
}

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './roles.component.html',
  styleUrl: './roles.component.css'
})
export class RolesComponent {
  private roleService = inject(RoleService);

  roles = signal<Role[]>([]);
  modules = signal<PermissionModule[]>([]);
  actions = signal<string[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  editing = signal<EditableRole | null>(null);
  saving = signal(false);
  formError = signal<string | null>(null);
  rowBusyId = signal<number | null>(null);

  isEditMode = computed(() => this.editing()?.roleId != null);

  constructor() {
    this.load();
    this.roleService.listPermissionModules().subscribe({
      next: (res) => {
        this.modules.set(res.modules);
        this.actions.set(res.actions);
      },
      error: () => {}
    });
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.roleService.list().subscribe({
      next: (roles) => {
        this.roles.set(roles);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load roles. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  startCreate(): void {
    this.formError.set(null);
    this.editing.set({
      roleId: null,
      roleName: '',
      roleCode: '',
      isExternal: false,
      isActive: true,
      grants: {}
    });
  }

  startEdit(role: Role): void {
    this.formError.set(null);
    this.editing.set({
      roleId: role.roleId,
      roleName: role.roleName,
      roleCode: role.roleCode,
      isExternal: role.isExternal,
      isActive: role.isActive,
      grants: {}
    });

    this.roleService.getById(role.roleId).subscribe({
      next: (detail: RoleDetail) => {
        const grants: Record<string, boolean> = {};
        for (const permission of detail.permissions) {
          grants[gridKey(permission.moduleCode, permission.permissionCode)] = permission.isGranted;
        }
        this.editing.update((current) => (current ? { ...current, grants } : current));
      },
      error: () => this.formError.set('Unable to load role permissions.')
    });
  }

  cancelEdit(): void {
    this.editing.set(null);
    this.formError.set(null);
  }

  updateRoleName(value: string): void {
    this.editing.update((current) => (current ? { ...current, roleName: value } : current));
  }

  updateRoleCode(value: string): void {
    this.editing.update((current) => (current ? { ...current, roleCode: value } : current));
  }

  updateIsExternal(value: boolean): void {
    this.editing.update((current) => (current ? { ...current, isExternal: value } : current));
  }

  updateIsActive(value: boolean): void {
    this.editing.update((current) => (current ? { ...current, isActive: value } : current));
  }

  isGranted(moduleCode: string, action: string): boolean {
    return this.editing()?.grants[gridKey(moduleCode, action)] ?? false;
  }

  toggleGrant(moduleCode: string, action: string, checked: boolean): void {
    this.editing.update((current) => {
      if (!current) return current;
      return { ...current, grants: { ...current.grants, [gridKey(moduleCode, action)]: checked } };
    });
  }

  save(): void {
    const draft = this.editing();
    if (!draft) return;

    if (!draft.roleName.trim()) {
      this.formError.set('Role name is required.');
      return;
    }
    if (!draft.roleId && !draft.roleCode.trim()) {
      this.formError.set('Role code is required.');
      return;
    }

    const permissions: RolePermission[] = Object.entries(draft.grants)
      .filter(([, isGranted]) => isGranted)
      .map(([key]) => {
        const [moduleCode, permissionCode] = key.split('::');
        return { moduleCode, permissionCode, isGranted: true };
      });

    this.formError.set(null);
    this.saving.set(true);

    const request$ = draft.roleId
      ? this.roleService.update(draft.roleId, {
          roleName: draft.roleName.trim(),
          isExternal: draft.isExternal,
          isActive: draft.isActive,
          permissions
        })
      : this.roleService.create({
          roleName: draft.roleName.trim(),
          roleCode: draft.roleCode.trim().toUpperCase(),
          isExternal: draft.isExternal,
          permissions
        });

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(null);
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        this.formError.set(err?.error?.error ?? err?.message ?? 'Unable to save role.');
      }
    });
  }

  deactivate(role: Role): void {
    if (!confirm(`Deactivate the "${role.roleName}" role? Users assigned to it will need to be reassigned first if any remain active.`)) {
      return;
    }
    this.rowBusyId.set(role.roleId);
    this.roleService.deactivate(role.roleId).subscribe({
      next: () => {
        this.rowBusyId.set(null);
        this.load();
      },
      error: (err) => {
        this.rowBusyId.set(null);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to deactivate role.');
      }
    });
  }
}
