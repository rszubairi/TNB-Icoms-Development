import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface NavItem {
  label: string;
  path?: string;
  disabled?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  navItems: NavItem[] = [
    { label: 'Dashboard', disabled: true },
    { label: 'User Management', path: '/admin/users' },
    { label: 'Roles & Permissions', path: '/admin/roles' },
    { label: 'Role Transfer Requests', path: '/admin/role-transfer-requests' },
    { label: 'Organisations & Stations', path: '/admin/organisations' },
    { label: 'Voltage & Equipment Types', path: '/admin/voltage-equipment' },
    { label: 'Equipment Directory', path: '/admin/equipment' },
    { label: 'Off-Point Management', path: '/admin/off-points' },
    { label: 'Dropdown Management', path: '/admin/dropdown-values' },
    { label: 'Project Management', path: '/admin/projects' },
    { label: 'Outage Type Configuration', path: '/admin/outage-type-rules' },
    { label: 'Outage Scheduling', path: '/admin/outage-scheduling' },
    { label: 'Authorisation Personnel', path: '/admin/authorisation-personnel' },
    { label: 'Outage Intake', disabled: true },
    { label: 'TOMS Docket', disabled: true },
    { label: 'Shift Handover', disabled: true }
  ];
}
