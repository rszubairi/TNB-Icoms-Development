# Technical Implementation Plan: TNB ICOMS 2.0 Enterprise System

## System Overview & Objectives
TNB ICOMS 2.0 is an enterprise-grade Integrated Outage Management System designed for Tenaga Nasional Berhad (TNB) Grid System Operation. It reinvents the legacy ICOMS system to provide a unified, secure, and fully auditable workflow for outage intake, technical study, multi-tier approvals, execution tracking, change requests, Single Line Diagrams (SLD), Commissioning Memos, and shift handovers across 4 key operational user groups:
1. **Requestor / Planner** (Module 2)
2. **TOMS / GNM** (Grid Network Management - Module 3)
3. **GNC** (Grid Network Control - Module 4)
4. **System Setup & Administration** (SysAdmin & GNM Admin - Module 1)

---

## Technical Stack & Architecture

### Backend Core Framework
- **Framework**: .NET 10.0 (backward compatible with .NET 8.0) ASP.NET Core Web API with RESTful endpoints and SignalR Hubs for real-time docket/handover synchronization.
- **ORM**: Entity Framework Core with Code-First Migrations and Fluent API configurations.
- **Database (RDS)**: Microsoft SQL Server 2022 (Azure SQL / AWS RDS / On-Premises SQL Server). Target database: `dbOutage`.
- **Email Service**: Resend API integration for transactional emails (user invitations, 2FA OTPs, workflow notifications) with abstraction allowing seamless switchover to TNB internal SMTP relay.
- **Caching**: `IMemoryCache` (in-memory) / Redis for zone listing and session metadata.
- **Background Jobs**: `IHostedService` / Hangfire for automated workflow time-based triggers (e.g. 2-week unconfirmed auto-close, 7-day DSO auto-agree, 31-day KIV timeout).

### Frontend Web Framework
- **Framework**: Angular 18/19 Standalone Components, Signals, RxJS, and Angular Router.
- **Design System & UI Components**: Tailwind CSS + Custom Enterprise styles strictly adhering to `Design/style_reference.css` and the HTML mockups.
- **State Management**: Angular Signals & RxJS for reactive state management.
- **File & PDF Viewer**: Integrated Canvas PDF Viewer / PDF.js for SLD schematics, Commissioning Memos, and Dataforms.

---

## Security, Authentication & Multi-Factor Authentication (2FA)

1. **Internal TNB Users**:
   - Authenticate via Active Directory / ADFS / Azure AD (Windows Auth / OIDC / LDAP via TNB ID).
   - Upon successful AD validation, 2FA prompt is **bypassed automatically**.
2. **External Users (GCU - Grid Connected Users)**:
   - Authenticate via ASP.NET Identity Membership (Email/Password).
   - 2FA via Time-based One-Time Password (TOTP via Authenticator app or Email OTP via Resend API) is **mandatory** for Production.
3. **Non-Production & Local Environments**:
   - Configurable 2FA bypass flag in `appsettings.Development.json` (`"Authentication:Bypass2FA": true`) to facilitate automated testing and local debugging.

---

## Database Architecture & Schemas

### Database Schemas
1. `auth`: Users, Roles, RolePermissions, UserGcuStations, RoleTransferRequests.
2. `config`: Zones, ZoneLocations, Organisations, Stations, VoltageLevel, EquipmentType, Equipment, ConflictingLines, DropdownValues, Projects, OutageTypeRules, AuthorisationPersonnel.
3. `dbo`: Outages, OutageAdditionalEquipment, OutagePic, OutageNotifyEmails, ChangeRequests, OutageOffPoints, GcuAcknowledgements, Authorisations, SingleLineDiagrams, CommissioningMemos.
4. `handover`: HandoverShifts, HandoverEntries.
5. `audit`: AuditLog, SavedReportFilters.

---

## MVP Phase 1: Authentication & User Administration

### Features for Immediate MVP Delivery:
1. **Administrator Login**:
   - Login page matching `Design/login.html` & `login.png`.
   - Support for both Master Admin credentials and Corporate AD / SSO simulation.
2. **User Management & Creation Portal (Module 1 - System Setup)**:
   - User directory listing with role and zone badges.
   - User creation form with fields: `TNB ID`, `Full Name`, `Email`, `Phone Number`, `Role`, `Zone`, `Organisation`, `GCU Type`, and `GCU Stations`.
   - Role & Permissions configuration interface.
   - User role transfer request approval system.
3. **Email Notification Engine (Resend API)**:
   - Email dispatch service using Resend API (`api.resend.com`) for user onboarding invitations and 2FA codes.
   - Pluggable `IEmailSender` interface structured for zero-effort transition to TNB corporate SMTP.

---

## Data Migration Plan (Legacy `dbOutage` on 52.74.111.85)

The ETL migration process reads from legacy `dbOutage` tables and populates the normalized ICOMS 2.0 schema:
- `TblGridZone` $\rightarrow$ `config.Zones`
- `tblorganisation_new` $\rightarrow$ `config.Organisations`
- `TblSubstation_new` $\rightarrow$ `config.Stations`
- `TblEquipment` $\rightarrow$ `config.Equipment` (Naming convention: `VoltageLevel - MVA - Name`)
- `TblUserProfile` $\rightarrow$ `auth.Users`
- `TblTxOutRequest` $\rightarrow$ `dbo.Outages`
- `TblAuthorisation` $\rightarrow$ `dbo.Authorisations`
- `TblProjectInfo` $\rightarrow$ `config.Projects`

Migration script location: `src/TnbIcoms.Migration/Migrate_dbOutage.sql`.

---

## Phased Implementation Roadmap

### Phase 1: MVP - System Setup & User Administration (Current Priority)
- Backend .NET Auth API with Resend email integration.
- Angular User Management and Login screens based on `Design/login.html` and `Design/index.html`.
- Administrator setup & user onboarding workflow.

### Phase 2: Outage Intake & Planner Agreement (Module 2)
- Outage creation form with dynamic zone/station/equipment cascades.
- Auto-classification rules (Planned, Unplanned, Emergency, Forced).
- Conflicting lines warning & redundancy checker.
- Planner Pending Review docket with bulk Agree/Disagree actions.
- Requestor Confirmation page.

### Phase 3: TOMS Docket, SLDs & Commissioning Memos (Module 3)
- 5-tab TOMS Approval Docket (Tomorrow, Sat-Sun-Mon, Next Week, Next Week Fri-Sat-Sun, Next Month).
- Outage study evaluation form & off-point selector.
- Single Line Diagram (SLD) drawing/revision workflow.
- Commissioning Memo interactive document builder.

### Phase 4: GNC Operational Control & Shift Handover (Module 4)
- Scheduled & Active Outage live tracking.
- Site Authorisation management (Taken-Active / Taken-Completed).
- Forced outage rapid creation.
- 3-shift logbook (Morning, Evening, Night) with live shift pass/lock and PDF/Excel export.

### Phase 5: Reporting, 5-Week Rolling Calendar & Final Polish
- 5-week continuous calendar by row and cell view.
- Statistical analytics (TYTOP, Repetitive outages, Planned availability).
- User custom favorite filter presets.
