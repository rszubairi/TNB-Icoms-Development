import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs/operators';

interface NavItem {
  label: string;
  path?: string;
  disabled?: boolean;
}

interface NavGroup {
  label: string;
  items: NavItem[];
  expanded: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit {
  topLevelItems: NavItem[] = [
    { label: 'Dashboard', disabled: true },
    { label: 'System Health', path: '/health-check' }
  ];

  navGroups: NavGroup[] = [
    {
      label: 'Administration',
      expanded: false,
      items: [
        { label: 'User Management', path: '/admin/users' },
        { label: 'Roles & Permissions', path: '/admin/roles' },
        { label: 'Role Transfer Requests', path: '/admin/role-transfer-requests' },
        { label: 'Organisations & Stations', path: '/admin/organisations' }
      ]
    },
    {
      label: 'Asset Configuration',
      expanded: false,
      items: [
        { label: 'Voltage & Equipment Types', path: '/admin/voltage-equipment' },
        { label: 'Equipment Directory', path: '/admin/equipment' },
        { label: 'Off-Point Management', path: '/admin/off-points' },
        { label: 'Dropdown Management', path: '/admin/dropdown-values' },
        { label: 'Transmission Lines', path: '/admin/transmission-lines' },
        { label: 'Conflicting Lines', path: '/admin/conflicting-lines' },
        { label: 'Linking Lines', path: '/admin/linking-lines' }
      ]
    },
    {
      label: 'Outage Management',
      expanded: false,
      items: [
        { label: 'Create Outage', path: '/outages/new' },
        { label: 'Outage Pending Review', path: '/outages/pending-review' },
        { label: 'Confirmation Page', path: '/outages/confirmation' },
        { label: 'Outage Pending Approval', path: '/outages/pending-approval' },
        { label: 'Data Repository', path: '/outages/repository' },
        { label: 'Change Requests', path: '/outages/change-requests' },
        { label: 'Outage Calendar', path: '/outages/calendar' },
        { label: 'Project Management', path: '/admin/projects' },
        { label: 'Outage Type Configuration', path: '/admin/outage-type-rules' },
        { label: 'Outage Scheduling', path: '/admin/outage-scheduling' },
        { label: 'Authorisation Personnel', path: '/admin/authorisation-personnel' },
        { label: 'Change Request Settings', path: '/admin/change-request-settings' },
        { label: 'Mnemonic List', path: '/admin/mnemonic' }
      ]
    },
    {
      label: 'Grid Network Control',
      expanded: false,
      items: [
        { label: 'GNC: Scheduled Outage', path: '/gnc/scheduled' },
        { label: 'GNC: Active Outages', path: '/gnc/active' },
        { label: 'GNC: Authorisation in Force', path: '/gnc/authorisation-in-force' },
        { label: 'GNC: Forced Outage', path: '/gnc/forced-outage' }
      ]
    },
    {
      label: 'Reports & Analytics',
      expanded: false,
      items: [
        { label: 'Statistics', path: '/statistics' },
        { label: 'Customised Reporting', path: '/reports' }
      ]
    },
    {
      label: 'Operations Tools',
      expanded: false,
      items: [
        { label: 'Shift Handover', path: '/handover' },
        { label: 'Single Line Diagrams', path: '/sld' },
        { label: 'Commissioning Memos', path: '/commissioning-memos' }
      ]
    },
    {
      label: 'System Logs',
      expanded: false,
      items: [
        { label: 'Error Logs', path: '/admin/error-logs' },
        { label: 'Email Logs', path: '/admin/email-logs' },
        { label: 'Email Templates', path: '/admin/email-templates' }
      ]
    }
  ];

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.expandGroupForUrl(this.router.url);

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => this.expandGroupForUrl(event.urlAfterRedirects));
  }

  toggleGroup(group: NavGroup): void {
    const expanding = !group.expanded;
    for (const g of this.navGroups) {
      g.expanded = g === group && expanding;
    }
  }

  private expandGroupForUrl(url: string): void {
    const match = this.navGroups.find((group) =>
      group.items.some((item) => item.path && url.startsWith(item.path))
    );
    for (const g of this.navGroups) {
      g.expanded = g === match;
    }
  }
}
