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
    { label: 'Outage Intake', disabled: true },
    { label: 'TOMS Docket', disabled: true },
    { label: 'Shift Handover', disabled: true }
  ];
}
