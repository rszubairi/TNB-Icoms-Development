# UAT Script 02 — Navigation Shell (Sidebar, Header, Route Guard)

**View(s):** `shared/layout/shell`, `shared/layout/sidebar`, `shared/layout/header`
**Applies to:** All authenticated routes

## Preconditions
- Logged in with a role that has visibility of multiple modules (Admin recommended for full-coverage pass).

---

### NAV-01 — Sidebar renders all module groups
**Steps:** Log in and observe the sidebar.
**Expected Result:** The following collapsible groups are present: Administration, Asset Configuration, Outage Management, Grid Network Control, Reports & Analytics, Operations Tools, System Logs. A top-level **Dashboard** item is also visible.

---

### NAV-02 — Dashboard item is disabled (known gap)
**Steps:** Click the **Dashboard** sidebar item.
**Expected Result (current behaviour):** Item is disabled and does not navigate anywhere (no route implemented). **Action for UAT:** confirm whether Dashboard is in scope for this cycle.

---

### NAV-03 — Expand/collapse group
**Steps:** Click a collapsed group header, e.g. "Administration".
**Expected Result:** Group expands to show its child links. Clicking again collapses it.

---

### NAV-04 — Active route auto-expands its group
**Steps:** Directly navigate (via URL) to a child route, e.g. `/admin/roles`.
**Expected Result:** The "Administration" group is automatically expanded and the "Roles & Permissions" link is highlighted as active (`routerLinkActive`).

---

### NAV-05 — Active link highlighting
**Steps:** Navigate between two different sidebar links.
**Expected Result:** Only the currently active link is visually highlighted at any time; highlight updates correctly on each navigation.

---

### NAV-06 — All listed links navigate to existing pages
**Steps:** Click through every enabled sidebar link listed below and confirm each loads its corresponding page without console errors:
- Administration: User Management, Roles & Permissions, Role Transfer Requests, Organisations & Stations
- Asset Configuration: Voltage & Equipment Types, Equipment Directory, Off-Point Management, Dropdown Management, Transmission Lines, Conflicting Lines, Linking Lines
- Outage Management: Create Outage, Pending Review, Confirmation Page, Pending Approval, Data Repository, Change Requests, Outage Calendar, Project Management, Outage Type Configuration, Outage Scheduling, Authorisation Personnel, Change Request Settings, Mnemonic List
- Grid Network Control: Scheduled Outage, Active Outages, Authorisation in Force, Forced Outage
- Reports & Analytics: Statistics, Customised Reporting
- Operations Tools: Shift Handover, Single Line Diagrams, Commissioning Memos
- System Logs: Error Logs, Email Logs, Email Templates

**Expected Result:** Every link routes correctly; no 404s or blank pages.

---

### NAV-07 — Header displays logged-in user
**Steps:** Observe the header after login.
**Expected Result:** Header shows the current user's name/identifier and provides access to Account and Logout actions.

---

### NAV-08 — Header "Account" link
**Steps:** Click the user menu in the header, select **Account**.
**Expected Result:** Navigates to `/account` (self-service profile page).

---

### NAV-09 — Role-based sidebar visibility
**Preconditions:** Test accounts for at least two different roles (e.g. Admin vs. GNC).
**Steps:** Log in as each role and compare sidebar contents.
**Expected Result:** Confirm with business requirements whether sidebar items should differ by role. Document actual behaviour: **flag as a defect if all modules are visible regardless of role**, since this was not clearly enforced in the code reviewed.

---

### NAV-10 — Direct URL access without permission
**Preconditions:** A non-admin role account.
**Steps:** While logged in as a non-admin role, directly navigate via URL to an admin-only route, e.g. `/admin/users`.
**Expected Result:** Access should be blocked/redirected (e.g. to an "unauthorized" page or back to a safe landing page). **Action for UAT:** verify this — codebase review found no explicit role/policy checks on `UsersController`; treat as high-priority defect if unauthenticated-only guarding is confirmed.

---

### NAV-11 — Responsive layout
**Steps:** Resize browser window / use a tablet-width viewport.
**Expected Result:** Sidebar collapses to a compact/hamburger form as appropriate ("responsive navigation sidebar" per commit history); content area remains usable without horizontal scroll.
