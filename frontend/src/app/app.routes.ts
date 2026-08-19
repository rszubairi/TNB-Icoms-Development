import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RolesComponent } from './features/admin/users/roles/roles.component';
import { RoleTransferRequestsComponent } from './features/admin/users/role-transfer-requests/role-transfer-requests.component';
import { UserFormComponent } from './features/admin/users/user-form/user-form.component';
import { UserListComponent } from './features/admin/users/user-list/user-list.component';
import { ShellComponent } from './shared/layout/shell/shell.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'admin/users' },
      { path: 'admin/users', component: UserListComponent },
      { path: 'admin/users/:id', component: UserFormComponent },
      { path: 'admin/roles', component: RolesComponent },
      { path: 'admin/role-transfer-requests', component: RoleTransferRequestsComponent },
      { path: '**', redirectTo: 'admin/users' }
    ]
  },
  { path: '**', redirectTo: 'admin/users' }
];
