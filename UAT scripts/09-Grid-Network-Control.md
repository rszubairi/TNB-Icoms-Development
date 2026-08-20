# UAT Script 09 — Grid Network Control (GNC)

**View(s):** Scheduled Outage (`/gnc/scheduled`), Active Outages (`/gnc/active`), Authorisation in Force (`/gnc/authorisation-in-force`), Forced Outage (`/gnc/forced-outage`)
**API:** `GET /api/gnc/scheduled`, `GET /api/gnc/active`, `GET /api/gnc/authorisation-in-force`, `POST /api/gnc/outages/{id}/take-active|complete|extend|close|not-taken|cancel`, `POST /api/gnc/forced-outages`

## Preconditions
- Logged in as a GNC (Grid Network Control) user.
- At least one outage exists with `GnmStatus = Approved` (a prerequisite for it to appear on the GNC Scheduled view — see OUT-19 in script 08).
- Authorisation Personnel reference data seeded (`/admin/authorisation-personnel`) and active.
- `GncStatus` values referenced throughout: `Taken-Active`, `Taken-Completed`, `Outage Closed`, `Outage Closed - Not Taken`, `Outage Closed - Cancelled by GNC`.

---

## Scheduled Outage

### GNC-01 — Scheduled list loads GNM-approved outages
**Priority:** High
**Steps:** Navigate to `/gnc/scheduled`.
**Expected Result:** `GET /api/gnc/scheduled` returns outages ready for GNC action (GNM-approved, not yet taken); list can be filtered by zone.

---

### GNC-02 — Take Active — required fields
**Priority:** High
**Steps:**
1. Click **Take Active** on a scheduled outage.
2. Attempt to submit with Authorisation No. or Authoriser (Personnel) blank.

**Expected Result:** Blocked client-side with "Authorisation No. and Authoriser are required." Both fields must be populated to proceed.

---

### GNC-03 — Take Active — successful submission
**Priority:** High
**Steps:** Open Take Active, enter Authorisation No., select an Authoriser from the (active-only) personnel dropdown, set Taken Active At (defaults to now), optional Remark; submit.
**Expected Result:** `POST /api/gnc/outages/{id}/take-active` succeeds; `GncStatus = Taken-Active`; outage moves from Scheduled to Active Outages and Authorisation in Force.

---

### GNC-04 — Not Taken — reason required
**Priority:** High
**Steps:** Click **Not Taken** on a scheduled outage; attempt submit without selecting a reason.
**Expected Result:** Blocked with "A reason is required." Reason list is sourced from `DropdownValue` category `NotTakenReason`.

---

### GNC-05 — Not Taken — successful submission
**Priority:** High
**Steps:** Select a Not Taken Reason, optional Remark, submit.
**Expected Result:** `POST /api/gnc/outages/{id}/not-taken` succeeds; outage's `GncStatus` becomes `Outage Closed - Not Taken`; `NotTakenReasonId` is persisted on the outage; outage removed from Scheduled list.

---

### GNC-06 — Cancel scheduled outage
**Priority:** Medium
**Steps:** Click **Cancel** on a scheduled outage, optionally add a Remark, submit.
**Expected Result:** `POST /api/gnc/outages/{id}/cancel` succeeds; `GncStatus = Outage Closed - Cancelled by GNC`; outage is removed from the Scheduled list and does not appear as active.

---

### GNC-07 — Scheduled zone filter
**Priority:** Low
**Steps:** Apply the zone filter (if present) on the Scheduled view.
**Expected Result:** Only outages in the selected zone are shown; matches `GET /api/gnc/scheduled?zoneId=`.

---

## Active Outages

### GNC-08 — Active list shows Taken-Active/Taken-Completed
**Priority:** High
**Steps:** Navigate to `/gnc/active`.
**Expected Result:** `GET /api/gnc/active` returns outages holding a live authorisation (`GncStatus` in Taken-Active or Taken-Completed); title "Active Outages".

---

### GNC-09 — Complete an active outage
**Priority:** High
**Steps:** Click **Complete** on a Taken-Active outage; set Taken Completed At (defaults to now), optional Remark; submit.
**Expected Result:** `POST /api/gnc/outages/{id}/complete` succeeds; `GncStatus = Taken-Completed`.

---

### GNC-10 — Extend an active outage
**Priority:** High
**Steps:** Click **Extend** on an active outage; set Extended To date/time, optional Remark; submit.
**Expected Result:** `POST /api/gnc/outages/{id}/extend` succeeds; `ExtendedEndAt` is updated on the outage; timeline status recalculates (see GNC-12).

---

### GNC-11 — Close a completed outage
**Priority:** High
**Steps:** Click **Close** on a Taken-Completed outage; confirm "Close outage {number}? This finalises it as Outage Closed."
**Expected Result:** `POST /api/gnc/outages/{id}/close` succeeds; `GncStatus = Outage Closed`; outage disappears from Active Outages and Authorisation in Force.

---

### GNC-12 — Timeline colour coding (on-time / extended / overdue)
**Priority:** Medium
**Steps:** Observe a Taken-Active outage whose planned window has passed without extension, one that has been extended, and one still within its window.
**Expected Result:** Row/timeline class is `status-overdue` when past the planned window with no extension, `status-extended` when an extension has been applied, `status-ontime` otherwise. Non-`Taken-Active` rows show no timeline class.

---

## Authorisation in Force

### GNC-13 — Authorisation in Force list scope
**Priority:** High
**Steps:** Navigate to `/gnc/authorisation-in-force`.
**Expected Result:** `GET /api/gnc/authorisation-in-force` returns only `GncStatus = Taken-Active` outages, colour-coded by whether they are within their scheduled window (same timeline logic as GNC-12).

---

### GNC-14 — Authorisation in Force is a live operational view
**Priority:** Low
**Steps:** Take an outage Active from Scheduled, then check it appears immediately in Authorisation in Force; Complete it and verify it drops off this view (moves to Active Outages only, per Taken-Completed) but remains until Closed.
**Expected Result:** View reflects `Taken-Active` outages in near-real time as GNC status transitions occur across the other screens.

---

## Forced Outage

### GNC-15 — Forced Outage form load and required fields
**Priority:** High
**Steps:** Navigate to `/gnc/forced-outage`; attempt Submit with Station, Voltage Level, Equipment Type, Primary Equipment, Job Type, Approver, or Description blank.
**Expected Result:** Blocked with "Station, voltage, equipment, job type, approver, and description are all required." Approver dropdown is sourced from active Authorisation Personnel.

---

### GNC-16 — Create Forced Outage — auto-approved on creation
**Priority:** High
**Steps:** Complete all required fields (Zone pre-filled from user, Station, Voltage Level, Equipment Type, Primary Equipment, optional Additional Equipment, Work Type, Has PTW, Planned Start/End defaulting to today, Job Type, Approver, Description, optional PICs); click Submit.
**Expected Result:** `POST /api/gnc/forced-outages` succeeds; success message "Forced outage {number} created and approved." — unlike the standard Outage Create flow, a Forced Outage is created and approved in one step (no separate Planner/GNM review chain), reflecting outage code `F`.

---

### GNC-17 — Forced Outage PIC management
**Priority:** Low
**Steps:** Add and remove PIC entries before submitting (same PIC widget as Outage Create — Name and Email required per PIC).
**Expected Result:** PIC list behaves identically to Outage Create (OUT-09); submitted PICs are attached to the forced outage record.

---

### GNC-18 — Navigate to Scheduled after creation
**Priority:** Low
**Steps:** After a successful Forced Outage creation, use the **Go to Scheduled** link/button.
**Expected Result:** Navigates to `/gnc/scheduled`. Because the forced outage is auto-approved, verify whether it actually needs to appear there — if it is immediately `Taken-Active`/closed-workflow, flag as a defect if it incorrectly shows as still "Scheduled".

---

## Role-Based Access & Edge Cases

### GNC-19 — Non-GNC role access restriction
**Priority:** High
**Steps:** As a non-GNC user (e.g. Requestor), attempt to access `/gnc/scheduled`, `/gnc/active`, `/gnc/forced-outage` directly by URL, and call the underlying `take-active`/`complete`/`close` endpoints directly.
**Expected Result:** Access should be denied/redirected; API calls rejected server-side. **Action for UAT:** flag as a defect if `GncController` (which only carries `[Authorize]`, no per-action role checks in code review) allows any authenticated user regardless of role to perform GNC actions.

---

### GNC-20 — Action on an outage no longer eligible (race condition)
**Priority:** Medium
**Steps:** Open two browser sessions on the same Scheduled outage; Take Active in one session, then attempt Take Active (or Not Taken/Cancel) on the same outage in the second, stale session.
**Expected Result:** Second action fails gracefully with a clear server error (e.g. "outage is not in a takeable state") rather than corrupting `GncStatus` or throwing an unhandled exception; list refreshes to the current state after the failed attempt.
