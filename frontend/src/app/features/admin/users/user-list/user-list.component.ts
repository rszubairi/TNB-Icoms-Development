import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { RoleService } from '../../../../core/services/role.service';
import { UserService } from '../../../../core/services/user.service';
import { UserListItem } from '../../../../core/models/user.model';
import { Role } from '../../../../core/models/role.model';
import { Zone } from '../../../../core/models/zone.model';
import { ZoneService } from '../../../../core/services/zone.service';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent {
  private userService = inject(UserService);
  private roleService = inject(RoleService);
  private zoneService = inject(ZoneService);

  users = signal<UserListItem[]>([]);
  roles = signal<Role[]>([]);
  zones = signal<Zone[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  searchTerm = signal('');
  roleFilter = signal('');
  zoneFilter = signal('');

  filteredUsers = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const role = this.roleFilter();
    const zone = this.zoneFilter();
    return this.users().filter((user) => {
      const matchesTerm =
        !term ||
        user.fullName.toLowerCase().includes(term) ||
        user.tnbId.toLowerCase().includes(term) ||
        user.email.toLowerCase().includes(term);
      const matchesRole = !role || user.roleId === role;
      const matchesZone = !zone || user.zoneId === zone;
      return matchesTerm && matchesRole && matchesZone;
    });
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.userService.list().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load users. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
    this.roleService.list().subscribe({ next: (roles) => this.roles.set(roles), error: () => {} });
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
  }
}
