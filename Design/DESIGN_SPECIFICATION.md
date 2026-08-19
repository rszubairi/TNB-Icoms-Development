# TNB ICOMS 2.0 - Design & Layout Theme Specification

> **Document Type**: Frontend Design System & Architecture Blueprint  
> **Target Audience**: AI Agents, Frontend Developers, UI/UX Engineers  
> **Source Platform**: TNB ICOMS 2.0 (Grid System Operator - Integrated Commissioning & Outage Management System)  
> **Reference Directory**: [`c:\Users\rszub\Documents\TNB Icoms\Design`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design)

---

## 1. Executive Summary & Application Overview

**TNB ICOMS 2.0** is an enterprise-grade Grid System Operations (GSO) web application designed for power utility grid operators, engineers, and management to coordinate, approve, authorize, and track transmission network outages and commissioning workflows.

The visual architecture follows a **modern, high-contrast, enterprise utility design aesthetic**:
- **Primary Color Identity**: Deep TNB Navy Blue combined with Grid System Operator (GSO) Emerald Green accenting.
- **Layout Model**: Responsive 2-column dashboard layout featuring a fixed/collapsible dark sidebar navigation and a sticky top enterprise header.
- **Typography & Precision**: `Outfit` display typography for headings, `Inter` for interface body text, and tabular monospace font formatting for system IDs, outage codes, and timestamp logs.
- **Component Design**: Clean card containers, high-visibility semantic status badges, interactive data tables, multi-step wizard forms, and live execution timelines.

---

## 2. Reference HTML Files Inventory

All 15 target web pages were accessed, fully rendered via browser engine, and saved locally inside the [`Design`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design) directory alongside the reference stylesheet [`style_reference.css`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/style_reference.css).

| # | Page Name | HTML Reference File | Live URL | Description / Functional Focus |
|---|---|---|---|---|
| 1 | **Login** | [`login.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/login.html) | `https://tnb-icoms-design.vercel.app/login` | Dual-pane split authentication page with Corporate SSO & credentials |
| 2 | **Dashboard** | [`index.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/index.html) | `https://tnb-icoms-design.vercel.app/` | Operations Overview with KPI stat cards, charts, and live activity feeds |
| 3 | **Outage Status** | [`outage-status.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-status.html) | `https://tnb-icoms-design.vercel.app/outage-status` | Status pipeline view (Scheduled, Active, Extended, Completed) |
| 4 | **Outage List** | [`outage-list.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-list.html) | `https://tnb-icoms-design.vercel.app/outage-list` | Master data table with search, filter tabs, sorting & row details |
| 5 | **Outage Creation** | [`outage-creation.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-creation.html) | `https://tnb-icoms-design.vercel.app/outage-creation` | Multi-step wizard form for registering planned outages |
| 6 | **Emergency Outage** | [`emergency-outage.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/emergency-outage.html) | `https://tnb-icoms-design.vercel.app/emergency-outage` | Urgent forced outage declaration & rapid dispatch list |
| 7 | **Change Request** | [`change-request.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/change-request.html) | `https://tnb-icoms-design.vercel.app/change-request` | Workflow table for outage modification & schedule revision requests |
| 8 | **Pending Review** | [`pending-review.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/pending-review.html) | `https://tnb-icoms-design.vercel.app/pending-review` | Approval queue with bulk review capabilities (Agree/Disagree) |
| 9 | **Confirmation** | [`confirmation.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/confirmation.html) | `https://tnb-icoms-design.vercel.app/confirmation` | Multi-tier sign-off table & confirmation checklist verification |
| 10 | **Authorization in Force** | [`authorization.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/authorization.html) | `https://tnb-icoms-design.vercel.app/authorization` | Live execution monitor with timers, extension requests & restore controls |
| 11 | **Authorization List** | [`authorization-list.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/authorization-list.html) | `https://tnb-icoms-design.vercel.app/authorization-list` | Historical and active operational permits log with export capabilities |
| 12 | **Data Repository** | [`data-repository.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/data-repository.html) | `https://tnb-icoms-design.vercel.app/data-repository` | Historical outage repository with multi-field search and document downloads |
| 13 | **Reports** | [`reports.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/reports.html) | `https://tnb-icoms-design.vercel.app/reports` | Analytical reporting suite with export features (Excel, PDF, CSV) |
| 14 | **Outage Calendar** | [`calendar.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/calendar.html) | `https://tnb-icoms-design.vercel.app/calendar` | Interactive grid scheduler (Month, Week, Day views) |
| 15 | **Off-Point List** | [`off-point-list.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/off-point-list.html) | `https://tnb-icoms-design.vercel.app/off-point-list` | Grid network status & off-point configuration management |

---

## 3. Design System Foundation & Theme Tokens

### 3.1 Color Palette

```
┌────────────────────────────────────────────────────────────────────────┐
│                        TNB ICOMS COLOR SYSTEM                          │
├──────────────────┬──────────────────────┬──────────────────────────────┤
│ Token Name       │ Color Hex / Value    │ Primary Application          │
├──────────────────┼──────────────────────┼──────────────────────────────┤
│ tnblue-dark      │ #013a61              │ Sidebar nav, Login left hero │
│ gso-blue         │ #024a7a              │ Secondary brand, dark badges │
│ gso-green        │ #23af4e              │ Primary accent, active status│
│ gso-green-dark   │ #1c8f3f              │ Primary hover, dark emerald  │
│ surface-bg       │ #f9fafb / #f3f4f6    │ Main page background         │
│ card-bg          │ #ffffff              │ Content containers & cards   │
├──────────────────┼──────────────────────┼──────────────────────────────┤
│ status-emerald   │ #10b981 / #d1fae5    │ Confirmed / Approved status  │
│ status-amber     │ #f59e0b / #fef3c7    │ In-Study / Pending review    │
│ status-rose      │ #f43f5e / #ffe4e6    │ Emergency / Forced / Cancelled│
│ status-blue      │ #3b82f6 / #dbeafe    │ Completed / Archived         │
└──────────────────┴──────────────────────┴──────────────────────────────┘
```

#### Color Usage Guidelines:
- **`bg-tnblue-dark` (`#013a61`)**: Applied to the main sidebar navigation container, mobile drawer backgrounds, and the left hero panel of the login page.
- **`gso-green` (`#23af4e`)**: Used for call-to-action buttons, active navigation item indicators, section subheaders (`//`), pulse dot animations, and positive metric indicators.
- **`gso-green-dark` (`#1c8f3f`)**: Used for hover states of emerald action buttons.
- **Border Accents**: Glassmorphism borders use `border-white/10` on dark panels and `border-gray-200` (`#e5e7eb`) on light cards.

---

### 3.2 Typography System

- **Primary Font**: `Inter, system-ui, -apple-system, sans-serif`
- **Display / Heading Font**: `Outfit, Inter, sans-serif`
- **Monospace Code/Data Font**: `ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace`

#### Hierarchy Standards:
| Role | Class Names | Size / Weight | Usage |
|---|---|---|---|
| **Page Title** | `text-2xl font-black tracking-tight text-gray-900` | 24px / 900 | Main `<h1>` top left header |
| **Section Header** | `text-lg font-bold text-gray-800` | 18px / 700 | Subsection titles (`//Operations Overview`) |
| **Card Title** | `text-sm font-semibold text-gray-700` | 14px / 600 | Card headers, table column titles |
| **Body Text** | `text-sm font-medium text-gray-600` | 14px / 500 | Main content text, table cell values |
| **Caption / ID** | `font-mono text-xs font-semibold text-gray-500` | 12px / 600 | Outage IDs (`OUT-2026-0891`), equipment codes |
| **Category Label**| `text-[9px] font-black uppercase tracking-widest text-white/30` | 9px / 900 | Sidebar nav section headers (`OVERVIEW`) |

---

## 4. Global Layout Architecture

The application layout employs a fixed shell structure across all authenticated pages (`index.html` through `off-point-list.html`).

```
+-------------------------------------------------------------------------------+
| TOP NAVBAR (navbar-enterprise - h-16)                                         |
| [Sidebar Toggle] // Page Title                     [Search] [Notify] [User Profile] |
+------------------+------------------------------------------------------------+
| SIDEBAR (w-72)   | MAIN CONTENT AREA (max-w-7xl mx-auto px-6 py-8)            |
|                  |                                                            |
|  [Logo & Badge]  | +--------------------------------------------------------+ |
|                  | | Page Header & Quick Action Buttons                     | |
|  -- OVERVIEW     | +--------------------------------------------------------+ |
|  - Dashboard     | | Stat KPI Cards Grid (4 Columns)                        | |
|  - Outage Status | +--------------------------------------------------------+ |
|                  | | Data Filter Bar & Search Input                         | |
|  -- REQUESTS     | +--------------------------------------------------------+ |
|  - Outage List   | | Data Table / Form Wizard / Timeline Card               | |
|  - Outage Create | |                                                        | |
|  - Emergency     | |                                                        | |
|  - Change Req    | |                                                        | |
|                  | +--------------------------------------------------------+ |
|  -- APPROVALS    | | Pagination / Status Bar                                | |
|  - Pending       | +--------------------------------------------------------+ |
|  - Confirmation  |                                                            |
|                  |                                                            |
|  -- AUTHORIZATION|                                                            |
|  - In Force      |                                                            |
|  - Auth List     |                                                            |
+------------------+------------------------------------------------------------+
```

### 4.1 Top Navigation Header (`navbar-enterprise`)
- **Container**: `h-16 bg-white border-b border-gray-200 sticky top-0 z-40`
- **Left Side**: Collapsible menu trigger button, section indicator `//` in `text-gso-green font-black`, and Page H1 Title.
- **Right Side**:
  - Global Search Input field with search icon.
  - Notification Bell icon with active ping indicator dot.
  - User Profile dropdown pill displaying avatar, name ("System Administrator"), role badge ("GSO Officer"), and status indicator.

### 4.2 Left Sidebar Navigation (`aside`)
- **Container**: `fixed inset-y-0 left-0 z-50 w-72 bg-tnblue-dark text-white border-r border-white/5 shadow-2xl`
- **Header Box**: Displays `GSO Logo` (`/images/gso-logo.png`), gradient divider, and active status pill (`ICOMS 2.0 Enterprise` with green pulsing dot).
- **Navigation Groups**:
  1. **Overview**: Dashboard (`/`), Outage Status (`/outage-status`)
  2. **Outage Requests**: Outage List (`/outage-list`), Outage Creation (`/outage-creation`), Emergency Outage (`/emergency-outage`), Change Request (`/change-request`)
  3. **Approvals**: Pending Review (`/pending-review`), Confirmation (`/confirmation`)
  4. **Authorization**: Authorization in Force (`/authorization`), Authorization List (`/authorization-list`)
  5. **System & Reports**: Data Repository (`/data-repository`), Reports (`/reports`), Calendar (`/calendar`), Off-Point List (`/off-point-list`)
- **Active Navigation Item**: Highlighting with `bg-gso-green/10 text-gso-green` and an emerald left border bar or glow.

---

## 5. UI Component Catalog & Specifications

### 5.1 KPI Metric Cards (Stat Cards)
- **Container**: `bg-white rounded-2xl p-6 border border-gray-100 shadow-sm hover:shadow-md transition-shadow`
- **Elements**:
  - Metric Title in top left (`text-xs font-semibold uppercase tracking-wider text-gray-400`)
  - Icon in top right enclosed in tinted circle (`w-10 h-10 rounded-xl flex items-center justify-center bg-gso-green/10 text-gso-green`)
  - Large Metric Value (`text-3xl font-black text-gray-900`)
  - Trend / Delta Indicator pill below value (`text-xs font-bold text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-full`)

### 5.2 Status Pills & Badges (`status-badge`)
All status pills use a rounded badge format with subtle border and fill:
- **`Confirmed` / `Approved`**: `bg-emerald-50 text-emerald-700 border border-emerald-200/60`
- **`In-Study` / `Pending`**: `bg-amber-50 text-amber-700 border border-amber-200/60`
- **`Emergency` / `Forced`**: `bg-rose-50 text-rose-700 border border-rose-200/60`
- **`Completed` / `Restored`**: `bg-blue-50 text-blue-700 border border-blue-200/60`
- **`Draft`**: `bg-gray-100 text-gray-600 border border-gray-200`

### 5.3 Data Tables
- **Container**: `bg-white rounded-2xl border border-gray-200 shadow-sm overflow-hidden`
- **Header Row (`<thead>`)**: `bg-gray-50/80 border-b border-gray-200 text-xs font-bold text-gray-500 uppercase tracking-wider`
- **Body Rows (`<tbody>`)**: `divide-y divide-gray-100 hover:bg-gray-50/50 transition-colors`
- **Action Buttons Column**: Contains row-level icon buttons (View Details, Edit, Approve, Reject, History).

### 5.4 Form Controls & Step Wizard
- **Step Wizard Header**: Horizontal step indicator (`1. Basic Info` -> `2. Equipment` -> `3. Timing` -> `4. Safety` -> `5. Submit`) connected via progress bar line.
- **Input Fields**: `bg-gray-50/50 border border-gray-200 rounded-xl px-4 py-2.5 text-sm text-gray-900 focus:bg-white focus:ring-2 focus:ring-gso-green/30 focus:border-gso-green`
- **Primary Submit Button**: `bg-gso-green hover:bg-gso-green-dark text-white font-bold px-6 py-3 rounded-xl shadow-lg shadow-gso-green/20 transition-all`

---

## 6. Page-by-Page Design & Component Breakdown

### 1. Login Page ([`login.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/login.html))
- **Layout**: 55/45 split screen. Left side is dark navy (`bg-tnblue-dark`) with glowing green hero effect (`bg-gso-hero`), grid pattern background, and GSO branding. Right side is a clean login form container.
- **Components**: Corporate SSO button ("Sign in with Corporate SSO"), username/password fields, "Keep me signed in" checkbox, and security compliance footer notice.

### 2. Dashboard ([`index.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/index.html))
- **Layout**: 4-column KPI metric summary row at top, followed by 2-column split (Left: Active Outages table & status map preview; Right: Activity feeds & Quick Creation shortcut buttons).
- **Key Features**: Quick outage creation trigger, audit log modal trigger, real-time activity stream.

### 3. Outage Status ([`outage-status.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-status.html))
- **Layout**: Filterable tab bar at top (Scheduled, Active, Extended, Complete) and grid of outage status cards.
- **Key Features**: Live status countdown timers, progress bars indicating completion percentage, emergency extension request buttons.

### 4. Outage List ([`outage-list.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-list.html))
- **Layout**: Comprehensive data table view with filter tabs (All, Confirmed, In-Study, Pending, Completed).
- **Key Features**: Export options (CSV, Excel), table search, pagination control, row checkbox selection for bulk operations.

### 5. Outage Creation ([`outage-creation.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-creation.html))
- **Layout**: Step wizard multi-page form layout.
- **Key Features**: Equipment lookup selector, date/time range picker, risk level selector, safety boundary checklist.

### 6. Emergency Outage ([`emergency-outage.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/emergency-outage.html))
- **Layout**: Red accented alert top banner (`bg-rose-50 border-rose-200`), urgent forced outage creation trigger, priority list table.
- **Key Features**: Rapid declaration form, emergency notification dispatcher toggle.

### 7. Change Request ([`change-request.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/change-request.html))
- **Layout**: Outage alteration table displaying original vs requested schedule changes.
- **Key Features**: Impact assessment score, change rationale input, revision history drawer.

### 8. Pending Review ([`pending-review.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/pending-review.html))
- **Layout**: Approval manager queue with bulk action control bar ("Bulk Agree", "Bulk Disagree").
- **Key Features**: Side-by-side diff review modal, disagreement comment box.

### 9. Confirmation ([`confirmation.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/confirmation.html))
- **Layout**: Multi-tier sign-off table requiring dual-authorization signatures.
- **Key Features**: Confirmation checklist modal, digital sign-off trigger button.

### 10. Authorization in Force ([`authorization.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/authorization.html))
- **Layout**: Live operational permit tracking view with real-time execution timeline cards.
- **Key Features**: "Manual Extension", "Authorize Restore", live countdown progress rings.

### 11. Authorization List ([`authorization-list.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/authorization-list.html))
- **Layout**: Table view of issued and expired authorizations.
- **Key Features**: Export Excel / PDF triggers, long-term authorization filter toggle.

### 12. Data Repository ([`data-repository.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/data-repository.html))
- **Layout**: Historical archive table with advanced multi-field search dropdown filters.
- **Key Features**: Single-click PDF/CSV document downloads, historical archive search.

### 13. Reports ([`reports.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/reports.html))
- **Layout**: Report builder parameter card (Date range picker, Region filter, Voltage class filter) above analytical output grid.
- **Key Features**: Export summary charts, detailed transmission outage summary report tables.

### 14. Outage Calendar ([`calendar.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/calendar.html))
- **Layout**: Grid calendar view with Month, Week, and Day toggle buttons.
- **Key Features**: Color-coded event blocks matching outage severity, interactive event click modal.

### 15. Off-Point List ([`off-point-list.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/off-point-list.html))
- **Layout**: Network configuration table listing substation off-points and operational status switches.
- **Key Features**: Off-point creation button, live operational state toggle switches.

---

## 7. Developer Implementation & Component Reusability Guide

When implementing new features or migrating this design system to React / Next.js / Vue:

1. **Tailwind CSS Configuration**: Add custom brand colors to `tailwind.config.js`:
   ```javascript
   theme: {
     extend: {
       colors: {
         tnblue: {
           dark: '#013a61',
           primary: '#024a7a',
           50: '#f0f7fc'
         },
         gso: {
           green: '#23af4e',
           'green-dark': '#1c8f3f',
           blue: '#024a7a'
         }
       },
       fontFamily: {
         sans: ['Inter', 'sans-serif'],
         display: ['Outfit', 'sans-serif'],
         mono: ['ui-monospace', 'monospace']
       }
     }
   }
   ```

2. **Component Mapping**:
   - Use [`index.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/index.html) as the template for the main layout wrapper (`AppLayout`).
   - Use [`outage-list.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-list.html) as the reference for building scalable `DataTable` components.
   - Use [`outage-creation.html`](file:///c:/Users/rszub/Documents/TNB%20Icoms/Design/outage-creation.html) as the blueprint for `StepWizard` form flows.

---
*End of Design Specification Document. Reference HTML files are located in `c:\Users\rszub\Documents\TNB Icoms\Design\`.*
