# UAT Script 14 — System Logs

**View(s):** Error Logs (`/admin/error-logs`), Email Logs (`/admin/email-logs`), Email Templates (`/admin/email-templates`)
**API:** `ErrorLogsController.cs`, `EmailLogsController.cs`, `EmailTemplatesController.cs`

## Preconditions
- Logged in as Admin/System Setup user.

---

## Error Logs

### LOG-01 — Error log list loads
**Priority:** Medium
**Steps:** Navigate to `/admin/error-logs`.
**Expected Result:** List of logged application errors displayed with timestamp, severity, source, and message/summary columns.

---

### LOG-02 — Filter error logs by date range / severity
**Steps:** Apply date range and/or severity filters.
**Expected Result:** List narrows correctly to matching entries.

---

### LOG-03 — View error log detail
**Steps:** Click into a specific error log entry.
**Expected Result:** Full error detail displayed (stack trace/context) without exposing this to non-admin roles elsewhere in the app.

---

### LOG-04 — Error log captures a real application error
**Steps:** Deliberately trigger a handled error condition in a lower environment (e.g. invalid API call), then check Error Logs.
**Expected Result:** A corresponding new entry appears in the log within a reasonable time, with accurate timestamp and context.

---

## Email Logs

### LOG-05 — Email log list loads
**Priority:** Medium
**Steps:** Navigate to `/admin/email-logs`.
**Expected Result:** List of sent emails displayed with recipient, subject, status (Sent/Failed/Pending), and timestamp.

---

### LOG-06 — Email log reflects a real triggered email
**Steps:** Trigger an action known to send an email (e.g. Role Transfer Request approval — see RTR-03, or user creation welcome email), then check Email Logs.
**Expected Result:** A corresponding entry appears with correct recipient/subject and a "Sent" status (via Resend API integration).

---

### LOG-07 — Failed email is logged with reason
**Steps:** Trigger an email to an invalid/unreachable address if testable in the lower environment.
**Expected Result:** Entry shows "Failed" status with an error reason/message, not silently dropped.

---

### LOG-08 — Filter email logs by status/date
**Steps:** Apply status and date filters.
**Expected Result:** List narrows correctly.

---

### LOG-09 — Resend a failed email (if supported)
**Steps:** From a Failed email log entry, trigger a resend action if available.
**Expected Result:** Email is re-attempted; log updates with a new attempt/status.

---

## Email Templates

### LOG-10 — Email templates list loads
**Priority:** Medium
**Steps:** Navigate to `/admin/email-templates`.
**Expected Result:** List of configured templates displayed (e.g. welcome email, role transfer notification, outage approval notification).

---

### LOG-11 — Edit an email template
**Steps:** Open a template, modify subject/body content (including any placeholder/merge fields), save.
**Expected Result:** Changes save successfully; subsequent emails of that type use the updated content — verify by triggering the relevant action and checking Email Logs (LOG-06) or a real inbox in a lower environment.

---

### LOG-12 — Placeholder/merge field validation
**Steps:** Save a template with an invalid/unsupported placeholder token.
**Expected Result:** Either validation prevents saving with an invalid token, or the system gracefully ignores/renders it blank rather than sending a broken email — confirm actual behaviour and flag as defect if broken emails are sent.

---

### LOG-13 — Preview email template
**Steps:** Use a **Preview** action if available on a template.
**Expected Result:** Rendered preview accurately reflects how the email will appear to recipients, with sample data substituted for placeholders.

---

### LOG-14 — Non-admin access restriction (all System Logs screens)
**Preconditions:** Login as non-admin role.
**Steps:** Attempt to navigate directly to each System Logs URL.
**Expected Result:** Access denied/redirected — these screens should not be accessible to non-admin roles, especially Error Logs (may expose sensitive technical detail).
