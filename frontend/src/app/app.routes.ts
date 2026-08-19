import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RolesComponent } from './features/admin/users/roles/roles.component';
import { RoleTransferRequestsComponent } from './features/admin/users/role-transfer-requests/role-transfer-requests.component';
import { OrganisationsComponent } from './features/admin/organisations/organisations.component';
import { VoltageEquipmentComponent } from './features/admin/voltage-equipment/voltage-equipment.component';
import { EquipmentComponent } from './features/admin/equipment/equipment.component';
import { OffPointsComponent } from './features/admin/off-points/off-points.component';
import { DropdownValuesComponent } from './features/admin/dropdown-values/dropdown-values.component';
import { ProjectsComponent } from './features/admin/projects/projects.component';
import { OutageTypeRulesComponent } from './features/admin/outage-type-rules/outage-type-rules.component';
import { OutageSchedulingComponent } from './features/admin/outage-scheduling/outage-scheduling.component';
import { AuthorisationPersonnelComponent } from './features/admin/authorisation-personnel/authorisation-personnel.component';
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
      { path: 'admin/organisations', component: OrganisationsComponent },
      { path: 'admin/voltage-equipment', component: VoltageEquipmentComponent },
      { path: 'admin/equipment', component: EquipmentComponent },
      { path: 'admin/off-points', component: OffPointsComponent },
      { path: 'admin/dropdown-values', component: DropdownValuesComponent },
      { path: 'admin/projects', component: ProjectsComponent },
      { path: 'admin/outage-type-rules', component: OutageTypeRulesComponent },
      { path: 'admin/outage-scheduling', component: OutageSchedulingComponent },
      { path: 'admin/authorisation-personnel', component: AuthorisationPersonnelComponent },
      { path: '**', redirectTo: 'admin/users' }
    ]
  },
  { path: '**', redirectTo: 'admin/users' }
];
