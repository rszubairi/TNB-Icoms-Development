# TNB ICOMS 2.0 — UAT Master Test Plan

## Purpose
This folder contains User Acceptance Test (UAT) scripts covering all features currently implemented in TNB ICOMS 2.0, as of 2026-08-20. Scripts are grouped by functional module, mirroring the application's sidebar navigation and backend controllers.

## Scope
Covers the Angular 18/19 frontend (`frontend/src/app`) and the ASP.NET Core Web API backend (`src/TnbIcoms.Api`). Test cases are written for a business/end user executing the application through the UI; API endpoints are noted for traceability, not for direct API testing.

## Test Script Index

| # | File | Module | Views Covered |
|---|------|--------|----------------|
| 1 | [01-Authentication-Login.md](01-Authentication-Login.md) | Authentication | Login, AD/SSO Login, Session/Logout |
| 2 | [02-Navigation-Shell.md](02-Navigation-Shell.md) | Shell / Navigation | Sidebar, Header, Route Guard |
| 3 | [03-User-Management.md](03-User-Management.md) | Administration | User List, User Create/Edit |
| 4 | [04-Roles-Permissions.md](04-Roles-Permissions.md) | Administration | Roles & Permissions |
| 5 | [05-Role-Transfer-Requests.md](05-Role-Transfer-Requests.md) | Administration | Role Transfer Requests |
| 6 | [06-Organisations-Stations.md](06-Organisations-Stations.md) | Administration | Organisations & Stations |
| 7 | [07-Asset-Configuration.md](07-Asset-Configuration.md) | Asset Configuration | Voltage & Equipment Types, Equipment Directory, Off-Point Management, Dropdown Management, Transmission Lines, Conflicting Lines, Linking Lines, Mnemonic List |
| 8 | [08-Outage-Management.md](08-Outage-Management.md) | Outage Management | Create Outage, Pending Review, Confirmation, Pending Approval, Data Repository, Change Requests, Outage Calendar, Project Management, Outage Type Configuration, Outage Scheduling, Authorisation Personnel, Change Request Settings |
| 9 | [09-Grid-Network-Control.md](09-Grid-Network-Control.md) | Grid Network Control | Scheduled Outage, Active Outages, Authorisation in Force, Forced Outage |
| 10 | [10-Statistics-Reports.md](10-Statistics-Reports.md) | Reports & Analytics | Statistics, Customised Reporting |
| 11 | [11-Handover.md](11-Handover.md) | Operations Tools | Shift Handover |
| 12 | [12-Single-Line-Diagrams.md](12-Single-Line-Diagrams.md) | Operations Tools | Single Line Diagrams |
| 13 | [13-Commissioning-Memos.md](13-Commissioning-Memos.md) | Operations Tools | Commissioning Memos |
| 14 | [14-System-Logs.md](14-System-Logs.md) | System Logs | Error Logs, Email Logs, Email Templates |
| 15 | [15-Account.md](15-Account.md) | Self-Service | Account (My Profile) |

## Test Case Format
Each script uses the following structure per test case:

- **Test ID** — unique identifier, e.g. `LOGIN-01`
- **Title** — short description
- **Preconditions** — required state/data/role before executing
- **Steps** — numbered UI actions
- **Expected Result** — observable pass criteria
- **API Reference** — backend endpoint(s) exercised (for traceability)
- **Priority** — High / Medium / Low

## Roles Referenced
- **Admin / System Setup** — full access to Administration & Asset Configuration modules
- **Requestor/Planner** — Outage Management module (create/submit outages, change requests)
- **TOMS/GNM** — Outage review/approval, SLD, Commissioning Memos
- **GNC** — Grid Network Control module, Handover
- **GCU (Grid Connected User)** — external user, email/password + mandatory 2FA login

## Known Gaps to Verify During UAT (flagged by codebase review)
1. **"Forgot password?"** link on the Login page is currently non-functional (`javascript:void(0)`) — confirm expected behaviour with business before sign-off.
2. **Dashboard** sidebar item is disabled (no route implemented) — confirm whether this is in scope for current UAT cycle.
3. **2FA/TOTP UI** for GCU/external users was not found wired into the Login component — verify whether 2FA is expected to be testable in this cycle.
4. **Granular role-based access** on `/admin/users` and other admin routes should be verified — confirm non-admin roles are correctly blocked, not just unauthenticated users.
5. **GCU Stations** multi-select on the User Form appears to use a hardcoded list (Station A/B/C/D) rather than live station data — verify against real station data.

## Environment / Preconditions for All Scripts
- Test environment URL: _[fill in — dev/staging URL]_
- Test accounts: _[fill in — one per role: Admin, Requestor/Planner, TOMS/GNM, GNC, GCU]_
- Browser(s): Chrome (latest), Edge (latest)
- Test data: seeded via `dbOutage` development seeder (zones, organisations, stations, equipment)

## Sign-off
| Module | Tester | Date | Result (Pass/Fail) | Notes |
|--------|--------|------|---------------------|-------|
| | | | | |
