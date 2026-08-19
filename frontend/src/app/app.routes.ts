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
import { ChangeRequestSettingsComponent } from './features/admin/change-request-settings/change-request-settings.component';
import { AccountComponent } from './features/account/account.component';
import { MnemonicComponent } from './features/admin/mnemonic/mnemonic.component';
import { TransmissionLinesComponent } from './features/admin/transmission-lines/transmission-lines.component';
import { ConflictingLinesComponent } from './features/admin/conflicting-lines/conflicting-lines.component';
import { LinkingLinesComponent } from './features/admin/linking-lines/linking-lines.component';
import { OutageCreateComponent } from './features/outages/outage-create/outage-create.component';
import { OutageListComponent } from './features/outages/outage-list/outage-list.component';
import { OutageDetailComponent } from './features/outages/outage-detail/outage-detail.component';
import { ChangeRequestReviewComponent } from './features/outages/change-request-review/change-request-review.component';
import { OutageCalendarComponent } from './features/outages/outage-calendar/outage-calendar.component';
import { StatisticsComponent } from './features/statistics/statistics.component';
import { ReportsComponent } from './features/reports/reports.component';
import { GncScheduledComponent } from './features/gnc/gnc-scheduled/gnc-scheduled.component';
import { GncActiveComponent } from './features/gnc/gnc-active/gnc-active.component';
import { ForcedOutageCreateComponent } from './features/gnc/forced-outage-create/forced-outage-create.component';
import { HandoverComponent } from './features/handover/handover.component';
import { SldListComponent } from './features/sld/sld-list/sld-list.component';
import { SldDetailComponent } from './features/sld/sld-detail/sld-detail.component';
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
      { path: 'admin/change-request-settings', component: ChangeRequestSettingsComponent },
      { path: 'account', component: AccountComponent },
      { path: 'admin/mnemonic', component: MnemonicComponent },
      { path: 'admin/transmission-lines', component: TransmissionLinesComponent },
      { path: 'admin/conflicting-lines', component: ConflictingLinesComponent },
      { path: 'admin/linking-lines', component: LinkingLinesComponent },
      { path: 'outages/new', component: OutageCreateComponent },
      { path: 'outages/pending-review', component: OutageListComponent, data: { mode: 'pendingReview' } },
      { path: 'outages/confirmation', component: OutageListComponent, data: { mode: 'confirmation' } },
      { path: 'outages/pending-approval', component: OutageListComponent, data: { mode: 'pendingApproval' } },
      { path: 'outages/repository', component: OutageListComponent, data: { mode: 'repository' } },
      { path: 'outages/change-requests', component: ChangeRequestReviewComponent },
      { path: 'outages/calendar', component: OutageCalendarComponent },
      { path: 'statistics', component: StatisticsComponent },
      { path: 'reports', component: ReportsComponent },
      { path: 'gnc/scheduled', component: GncScheduledComponent },
      { path: 'gnc/active', component: GncActiveComponent, data: { mode: 'active' } },
      { path: 'gnc/authorisation-in-force', component: GncActiveComponent, data: { mode: 'authorisationInForce' } },
      { path: 'gnc/forced-outage', component: ForcedOutageCreateComponent },
      { path: 'handover', component: HandoverComponent },
      { path: 'sld', component: SldListComponent },
      { path: 'sld/:id', component: SldDetailComponent },
      { path: 'outages/:id', component: OutageDetailComponent },
      { path: '**', redirectTo: 'admin/users' }
    ]
  },
  { path: '**', redirectTo: 'admin/users' }
];
