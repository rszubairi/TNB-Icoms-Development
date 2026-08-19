import { Component, inject, signal } from '@angular/core';
import { RoleService } from '../../../../core/services/role.service';
import { Role } from '../../../../core/models/role.model';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [],
  templateUrl: './roles.component.html',
  styleUrl: './roles.component.css'
})
export class RolesComponent {
  private roleService = inject(RoleService);

  roles = signal<Role[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  constructor() {
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
}
