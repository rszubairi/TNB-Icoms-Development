# UAT Script 10 — Statistics & Reports

**View(s):** Statistics (`/statistics`), Customised Reporting (`/reports`)
**API:** `StatisticsController.cs`, `ReportsController.cs`

## Preconditions
- Logged in with a role that has access to Reports & Analytics (e.g. Admin, TOMS/GNM, GNC).
- Historical outage/handover/SLD data exists in the test environment to produce meaningful statistics.

---

## Statistics

### STAT-01 — Statistics dashboard loads
**Priority:** High
**Steps:** Navigate to `/statistics`.
**Expected Result:** Dashboard loads with summary metrics/charts (e.g. outage counts by type/status/zone, trend over time) without errors.

---

### STAT-02 — Filter statistics by date range
**Steps:** Apply a date range filter (e.g. last 30 days, custom range).
**Expected Result:** Charts/metrics update to reflect only data within the selected range.

---

### STAT-03 — Filter statistics by zone/station/outage type
**Steps:** Apply available filters individually and in combination.
**Expected Result:** Data updates correctly for each filter and combination; totals are consistent with underlying records (spot-check against Data Repository, script 08).

---

### STAT-04 — Statistics with no matching data
**Steps:** Apply a filter combination expected to return no records (e.g. a future date range).
**Expected Result:** Dashboard displays an empty/zero state gracefully, not an error or broken chart.

---

### STAT-05 — Chart/metric drill-down (if supported)
**Steps:** Click into a chart segment or summary tile.
**Expected Result:** Navigates to or displays the underlying detailed records matching that segment.

---

## Customised Reporting

### RPT-01 — Report builder loads
**Priority:** High
**Steps:** Navigate to `/reports`.
**Expected Result:** Report configuration UI loads (report type selector, filters, date range, output format).

---

### RPT-02 — Generate a report with default filters
**Steps:** Select a report type, leave default filters, click **Generate**.
**Expected Result:** Report is generated and displayed/downloaded successfully within a reasonable time.

---

### RPT-03 — Generate a report with custom filters
**Steps:** Configure zone, station, outage type, and date range filters, generate the report.
**Expected Result:** Report content reflects only the filtered dataset; row/record counts match expectations.

---

### RPT-04 — Export report as PDF
**Steps:** Generate a report, select PDF export/download.
**Expected Result:** A correctly formatted PDF file downloads with matching data and readable layout (headers, pagination, no truncated columns).

---

### RPT-05 — Export report as Excel
**Steps:** Generate a report, select Excel export/download.
**Expected Result:** A valid .xlsx file downloads; opens correctly in Excel; column headers and data match the on-screen report.

---

### RPT-06 — Report generation with no matching data
**Steps:** Apply filters that yield zero records, generate the report.
**Expected Result:** Report generates gracefully showing "no data" rather than erroring or producing a blank/corrupt file.

---

### RPT-07 — Large dataset performance
**Steps:** Generate a report spanning a large date range/all zones.
**Expected Result:** Report completes within an acceptable time (define SLA with business, e.g. <30s) or shows a progress/loading indicator; no browser timeout or crash.

---

### RPT-08 — Role-based access to Statistics/Reports
**Preconditions:** Login as a role without Reports & Analytics access (if such a role exists per business rules).
**Steps:** Attempt to navigate to `/statistics` and `/reports` directly.
**Expected Result:** Access denied/redirected as per business rules; confirm actual expected access matrix with business before marking pass/fail.
