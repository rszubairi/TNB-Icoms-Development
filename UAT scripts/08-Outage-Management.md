# UAT Script 08 — Outage Management

**View(s):** Create Outage (`/outages/new`), Pending Review (`/outages/pending-review`), Confirmation (`/outages/confirmation`), Pending Approval (`/outages/pending-approval`), Data Repository (`/outages/repository`), Outage Detail (`/outages/:id`), Change Request Review (`/outages/change-requests`), Outage Calendar (`/outages/calendar`), Project Management (`/admin/projects`), Outage Type Configuration (`/admin/outage-type-rules`), Outage Scheduling (`/admin/outage-scheduling`), Authorisation Personnel (`/admin/authorisation-personnel`), Change Request Settings (`/admin/change-request-settings`)
**API:** `OutagesController.cs`, `ChangeRequestsController.cs`, `ProjectsController.cs`, `OutageTypeRulesController.cs`, `OutageScheduleWindowsController.cs`, `AuthorisationPersonnelController.cs`

## Preconditions
- Logged in as Requestor/Planner (or Admin) with a Zone assigned.
- Reference data configured: Zones, Stations, Voltage Levels, Equipment Types, Equipment, Job Type/Sequence/Restoration dropdown values, at least one active Outage Type Rule per Work Type/Voltage combination, and (for non-"Routine Maintenance" jobs) at least one active Project.
- **Known gap:** most workflow actions (`agree`, `confirm`, `gnm-approve`, etc.) only require generic `[Authorize]` server-side — no explicit role checks beyond outage creation auto-status logic. Treat OUT-ROLE-* cases as high-priority defects to confirm.

---

## Create Outage

### OUT-01 — Create outage — cascading field dependencies
**Priority:** High
**Steps:**
1. Navigate to `/outages/new`. Confirm Zone defaults to the current user's zone.
2. Select a Station (only stations in the chosen Zone are selectable).
3. Select a Voltage (Equipment Type/Primary Equipment reset).
4. Select Equipment Type (Primary Equipment options load, filtered by type+station).
5. Select Primary Equipment.

**Expected Result:** Each dropdown is disabled until its predecessor is chosen; changing Zone clears Station; changing Voltage resets Equipment Type, Primary Equipment, and Additional Equipment selections.

---

### OUT-02 — Save as Draft with minimum required fields
**Steps:** Complete Station, Voltage, Equipment Type, Primary Equipment, Job Type, Start Date, End Date only (leave Description blank), click **Save as Draft**.
**Expected Result:** `POST /api/outages` succeeds with `RequestorStatus="Draft"`, `PlannerStatus=null`, `GnmStatus="Pending"`. Description is not required for a draft.

---

### OUT-03 — Save as Draft — required field validation
**Steps:** Attempt to save a draft with Station, Voltage, Equipment Type, Primary Equipment, Job Type, Start Date, or End Date left blank (test individually).
**Expected Result:** Blocked with error "Zone, station, voltage, equipment, job type, and dates are required."

---

### OUT-04 — Submit Outage Request — Description required
**Steps:** Complete all minimum fields, leave Description blank, click **Submit Outage Request**.
**Expected Result:** Blocked with "Description is required to submit." Saving as Draft with the same data (OUT-02) should still succeed.

---

### OUT-05 — Submit — Project required for non-Routine-Maintenance job types
**Steps:** Select a Job Type other than "Routine Maintenance", leave Project blank, complete other required fields including Description, click **Submit Outage Request**.
**Expected Result:** Blocked with "A project is required for this job type." Selecting "Routine Maintenance" should hide/waive the Project requirement.

---

### OUT-06 — Successful submission — status assignment (normal requestor, non-emergency)
**Preconditions:** Logged in as a standard Requestor (not Planner/GNM/GNM_ADMIN role); outage dates/voltage do not classify as Emergency per Outage Type Rules.
**Steps:** Complete all required fields including Description and Project (if applicable), click **Submit Outage Request**.
**Expected Result:** Outage created with `RequestorStatus="Pending"`, `PlannerStatus=null`, `GnmStatus="Pending"`. Outage appears in **Pending Review**.

---

### OUT-07 — Successful submission — Emergency outage auto-agreed/confirmed
**Preconditions:** Dates/voltage chosen classify the outage as "Emergency" per configured Outage Type Rules (e.g. very short lead time).
**Steps:** Submit an outage meeting Emergency criteria.
**Expected Result:** `PlannerStatus="Agreed"`, `RequestorStatus="Confirmed"` set automatically on creation — outage skips Pending Review/Confirmation and appears directly in **Pending Approval**.

---

### OUT-08 — Successful submission — created by Planner/GNM/GNM_ADMIN role
**Preconditions:** Logged in as a user with Planner, GNM, or GNM_ADMIN role.
**Steps:** Submit a non-emergency outage.
**Expected Result:** `PlannerStatus="Agreed"` is set automatically (RequestorStatus remains "Pending") — outage skips Pending Review and appears directly in **Confirmation**.

---

### OUT-09 — Outage Type auto-classification — no matching rule
**Preconditions:** Outage Type Rules configured such that no rule covers the chosen Work Type + Voltage + lead-time combination (see script's Outage Type Configuration section to engineer this gap).
**Steps:** Attempt to submit/save an outage with a lead time that falls in the gap.
**Expected Result:** Save fails with an error directing the user to Outage Type Configuration; no outage record is created.

---

### OUT-10 — Redundant outage block
**Preconditions:** An existing non-closed outage on the same Primary Equipment + Job Type with overlapping dates.
**Steps:** Attempt to create a new outage with the same equipment, job type, and overlapping dates.
**Expected Result:** Blocked with "An outage already exists for this equipment, job type, and overlapping dates."

---

### OUT-11 — KIV warning (non-blocking)
**Preconditions:** An existing outage on the same equipment already set to `GnmStatus="KIV"`.
**Steps:** Create a new outage on the same equipment with non-overlapping-enough parameters to pass validation.
**Expected Result:** Outage is created successfully, but a non-blocking warning is returned/displayed suggesting a Change Request instead be used.

---

### OUT-12 — Not Taken lock
**Preconditions:** The zone has an outage with `GncStatus="Outage Closed - Not Taken"` and a null `NotTakenReasonId`.
**Steps:** Attempt to create any new outage in that zone.
**Expected Result:** Creation is blocked entirely until the Not Taken record is resolved (has a reason recorded).

---

### OUT-13 — End date must be after start date
**Steps:** Set Date End earlier than or equal to Date Start, attempt to save.
**Expected Result:** Blocked with a validation error.

---

### OUT-14 — Additional Equipment multi-select
**Steps:** Choose an Additional Equipment Type, select 2+ items from the Additional Equipment checkbox list, save.
**Expected Result:** All selected additional equipment IDs are persisted and visible on the Outage Detail page.

---

### OUT-15 — Add/remove PIC entries
**Steps:** Add two PIC entries (Name + Email required, Phone optional), remove one, submit.
**Expected Result:** "Add" is blocked until both Name and Email are non-empty; only the remaining PIC(s) are saved; PIC(s) receive a submission notification email (cross-check Email Logs, script 14).

---

### OUT-16 — Success screen actions
**Steps:** After a successful submission, use **Create Another** and **Go to Data Repository** buttons.
**Expected Result:** "Create Another" reloads a blank `/outages/new` form; "Go to Data Repository" navigates to `/outages/repository`.

---

## Pending Review (Planner review)

### OUT-17 — Pending Review list scoping
**Priority:** High
**Steps:** Navigate to `/outages/pending-review`.
**Expected Result:** Only outages with `RequestorStatus="Pending" && PlannerStatus==null` are listed.

---

### OUT-18 — Agree (single row)
**Steps:** Click **Agree** on a listed outage.
**Expected Result:** `POST /outages/{id}/agree` succeeds (valid only when RequestorStatus=="Pending" && PlannerStatus==null); `PlannerStatus` becomes `"Agreed"`; outage moves to **Confirmation**.

---

### OUT-19 — Disagree (single row)
**Steps:** Click **Disagree** on a listed outage.
**Expected Result:** `POST /outages/{id}/disagree` succeeds; `RequestorStatus` reverts to `"Draft"` (PlannerStatus stays null); outage is sent back to the requestor and disappears from Pending Review; requestor can reopen/edit and resubmit.

---

### OUT-20 — Bulk Agree / Bulk Disagree
**Steps:** Select multiple rows via checkboxes, use the bulk action bar to Agree (or Disagree) all selected.
**Expected Result:** `POST /outages/bulk/agree` (or `/bulk/disagree`) processes all selected; any per-item failures are reflected in a partial `errors[]` response and surfaced to the user without silently succeeding.

---

### OUT-21 — Select All checkbox and count
**Steps:** Use the "Select All" checkbox.
**Expected Result:** All visible rows are selected; the bulk action bar shows the correct selected count; bulk buttons are disabled when selection is empty.

---

## Confirmation Page

### OUT-22 — Confirmation list scoping
**Steps:** Navigate to `/outages/confirmation`.
**Expected Result:** Only outages with `PlannerStatus="Agreed" && RequestorStatus="Pending"` are listed.

---

### OUT-23 — Confirm (single row)
**Steps:** Click **Confirm**.
**Expected Result:** `POST /outages/{id}/confirm` succeeds (valid only when PlannerStatus=="Agreed" && RequestorStatus=="Pending"); `RequestorStatus` becomes `"Confirmed"`; outage moves to **Pending Approval**.

---

### OUT-24 — Reject (single row) — irreversible hard delete
**Steps:** Click **Reject**, confirm the browser dialog warning the action cannot be undone.
**Expected Result:** `POST /outages/{id}/reject` succeeds; the outage record is hard-deleted (`IsDeleted=true`); it disappears from all lists including Data Repository. **Confirm this matches intended business behaviour** — a hard delete at this stage is a significant business rule to validate explicitly with stakeholders.

---

### OUT-25 — Bulk Confirm / Bulk Reject
**Steps:** Select multiple rows, use bulk Confirm and bulk Reject actions.
**Expected Result:** `POST /outages/bulk/confirm` / `/bulk/reject` process correctly; reject warns about irreversibility before proceeding.

---

## Pending Approval (GNM)

### OUT-26 — Pending Approval list scoping
**Steps:** Navigate to `/outages/pending-approval`.
**Expected Result:** Only outages with `RequestorStatus="Confirmed" && PlannerStatus="Agreed" && GnmStatus in [Pending, Under-Study]` are listed.

---

### OUT-27 — Approve (single row)
**Steps:** Click **Approve**.
**Expected Result:** `POST /outages/{id}/gnm-approve` succeeds; `GnmStatus` becomes `"Approved"`, `ApprovedById` recorded; outage disappears from Pending Approval and becomes available in GNC's Scheduled Outage (script 09, GNC-01).

---

### OUT-28 — Disapprove (single row)
**Steps:** Click **Disapprove**, confirm the browser dialog.
**Expected Result:** `POST /outages/{id}/gnm-disapprove` succeeds (valid from GnmStatus in [Pending, Under-Study, KIV]); `GnmStatus` becomes `"Disapproved"`.

---

### OUT-29 — Bulk Approve / Bulk Disapprove
**Steps:** Select multiple rows, use bulk Approve and bulk Disapprove.
**Expected Result:** Both bulk actions succeed correctly with partial-error handling as in OUT-20.

---

## Data Repository (read-only)

### OUT-30 — Data Repository is read-only
**Steps:** Navigate to `/outages/repository`.
**Expected Result:** Lists all outages with `PlannerStatus="Agreed" && RequestorStatus="Confirmed"` within the user's zone; no checkboxes, bulk action bar, or row action buttons are shown — only a **View** link per row.

---

## Outage Detail — GNM Study & Approval

### OUT-31 — Update study notes without notifying
**Steps:** Open an outage detail page, edit Justification/Highlights/Remark/Under-Study Notes, click **Update**.
**Expected Result:** `PUT /outages/{id}/study?notify=false` succeeds; fields persist; no PIC email is sent (cross-check Email Logs).

---

### OUT-32 — Update study notes with notification
**Steps:** Edit study fields, click **Update & Notify**.
**Expected Result:** `PUT /outages/{id}/study?notify=true` succeeds; PIC(s) receive an "updated by GNM" notification email.

---

### OUT-33 — Start Study
**Preconditions:** Outage with `GnmStatus="Pending"`.
**Steps:** Click **Start Study**.
**Expected Result:** `POST /outages/{id}/study/start` succeeds; `GnmStatus` becomes `"Under-Study"`; button is hidden thereafter.

---

### OUT-34 — Start Study invalid from non-Pending state
**Steps:** Attempt Start Study on an outage not in `GnmStatus="Pending"` (button should not even be visible — verify via direct API call or stale page state).
**Expected Result:** Action rejected server-side with a clear error if attempted.

---

### OUT-35 — Set KIV
**Preconditions:** Outage with `GnmStatus` in Pending or Under-Study.
**Steps:** Click **Set KIV**.
**Expected Result:** `POST /outages/{id}/kiv` succeeds; `GnmStatus` becomes `"KIV"`.

---

### OUT-36 — GNM Approve from detail page
**Preconditions:** `RequestorStatus="Confirmed" && PlannerStatus="Agreed" && GnmStatus in [Pending, Under-Study, KIV]`.
**Steps:** Click **Approve** on the outage detail page.
**Expected Result:** Same result as OUT-27; button only visible when the precondition holds (`canGnmApprove()`).

---

### OUT-37 — GNM Disapprove from detail page
**Steps:** Click **Disapprove** when GnmStatus is Pending/Under-Study/KIV.
**Expected Result:** Same result as OUT-28.

---

### OUT-38 — Revert to Under-Study
**Preconditions:** `GnmStatus="Approved"` and `GncStatus` is null or `"Outage Closed - Not Taken"`.
**Steps:** Click **Revert to Under-Study**, confirm the dialog.
**Expected Result:** `POST /outages/{id}/gnm-revert` succeeds; `GnmStatus` returns to `"Under-Study"`; `ApprovedById` is cleared; button hidden.

---

### OUT-39 — Revert blocked once GNC has taken the outage active
**Preconditions:** `GnmStatus="Approved"` and `GncStatus="Taken-Active"` (or any GNC status other than null/Not-Taken).
**Steps:** Attempt to revert (button should not be visible; verify via direct call if possible).
**Expected Result:** Server rejects with "This outage already has GNC activity and cannot be reverted."

---

## Outage Detail — Change Requests

### OUT-40 — Submit a Change Request — schedule change
**Preconditions:** Outage `GnmStatus` is not "Approved" and neither status starts with "Outage Closed"; no existing Pending batch for this outage.
**Steps:** Click **+ Submit Change Request**, set a New Date/Time (both start and end), enter a Reason, submit.
**Expected Result:** `POST /api/change-requests` succeeds; a new Pending batch is created with a Schedule field row (old→new); "+ Submit Change Request" button is hidden and hint "A change request is already pending review." is shown.

---

### OUT-41 — Submit a Change Request — voltage or equipment change
**Steps:** Submit a Change Request selecting only New Voltage or only New Equipment (leave dates as "No change"), with a Reason.
**Expected Result:** Batch is created with only the changed field(s) as rows.

---

### OUT-42 — Change Request requires at least one changed field
**Steps:** Enter only a Reason, leave Date/Time, Voltage, and Equipment all unchanged, submit.
**Expected Result:** Blocked with "Change at least one of: date/time, voltage, or equipment."

---

### OUT-43 — Change Request requires a Reason
**Steps:** Change a field (e.g. New Voltage) but leave Reason blank, submit.
**Expected Result:** Blocked — Reason is required.

---

### OUT-44 — Cannot submit Change Request on an Approved outage
**Preconditions:** Outage `GnmStatus="Approved"`.
**Steps:** Observe the Change Request section on the detail page.
**Expected Result:** "+ Submit Change Request" is hidden; a hint indicates a manual GNM request is needed instead (`canRequestChange()` is false).

---

### OUT-45 — Cannot submit a second Change Request while one is Pending
**Preconditions:** A Pending batch already exists for the outage.
**Steps:** Attempt to submit a new Change Request via direct API call (UI button is hidden).
**Expected Result:** Server rejects with an appropriate error.

---

### OUT-46 — Selecting the same value as current produces no change row
**Steps:** Open Change Request form, select New Voltage equal to the outage's current voltage, submit with a Reason and no other changes.
**Expected Result:** Confirm actual behaviour — code review suggests this silently produces zero diff rows and may fail with "Select at least one field to change." even though a value was "selected." Log as a defect/UX issue if the user isn't clearly told why submission failed.

---

### OUT-47 — Batch history display
**Steps:** View an outage with one or more historical Change Request batches (Approved/Rejected/Pending).
**Expected Result:** Each batch shows status badge, requestedAt/By, field rows (old→new), Reason, and (if rejected) the GNM's review comment.

---

## Change Request Review (GNM)

### OUT-48 — Pending Change Requests list
**Priority:** High
**Steps:** Navigate to `/outages/change-requests`.
**Expected Result:** Only batches with `Status="Pending"` are listed, each linking to its outage and showing field rows and reason.

---

### OUT-49 — Approve a Change Request batch
**Steps:** Click **Approve**, confirm the dialog ("Changes apply immediately").
**Expected Result:** `POST /api/change-requests/{batchId}/approve` succeeds; all rows must be Pending (else fails "already been actioned"); each changed field is applied directly to the Outage record; all rows marked Approved with ApprovedBy/At stamped; outage's UpdatedAt/By stamped; verify the outage detail page now reflects the new schedule/voltage/equipment.

---

### OUT-50 — Reject a Change Request batch with comment
**Steps:** Click **Reject**, enter an optional Comment, click **Confirm Reject**.
**Expected Result:** `POST /api/change-requests/{batchId}/reject` succeeds; all rows set to Rejected with the ReviewComment stored; comment is visible to the requestor on the outage's batch history (OUT-47).

---

### OUT-51 — Reject can be cancelled
**Steps:** Click **Reject**, then click **Cancel** instead of confirming.
**Expected Result:** No change is made to the batch; it remains Pending.

---

### OUT-52 — Cannot re-action an already-actioned batch
**Steps:** Attempt to Approve or Reject a batch whose rows are no longer all Pending (e.g. race condition — simulate by having two testers open the same batch).
**Expected Result:** Second action fails with an "already been actioned" error; no double-application of changes occurs.

---

## Outage Calendar

### OUT-53 — Calendar loads current month grid
**Priority:** Medium
**Steps:** Navigate to `/outages/calendar`.
**Expected Result:** A 5-week (35-day), Monday-first grid renders for the current month; outages overlapping each visible day are shown as status chips.

---

### OUT-54 — Navigate Prev / Next / Today
**Steps:** Click **Prev month**, then **Next month** twice, then **Today**.
**Expected Result:** Grid reloads outages for each new visible range; "Today" always returns to the grid containing the current date; month label updates correctly (e.g. "August 2026").

---

### OUT-55 — Status chip colour mapping
**Steps:** View outages with a variety of statuses on the calendar, including at least one with a status not in the explicit chip map (e.g. "Under-Study", "KIV", or any "Outage Closed - ..." variant).
**Expected Result:** Pending/Agreed/Confirmed/Approved/Rejected/Closed map to their distinct chip colours. **Action for UAT:** confirm whether statuses like Under-Study/KIV/Outage Closed variants are expected to have their own distinct chips — code review found these fall back to the generic "chip-pending" style, which may be visually misleading. Log as a defect if a distinct visual is expected.

---

### OUT-56 — Click a day with outages
**Steps:** Click a calendar day cell that has one or more outages.
**Expected Result:** Day is selected and its outage details are shown (e.g. in a detail panel below the grid).

---

### OUT-57 — Click a day with no outages
**Steps:** Click an empty day cell.
**Expected Result:** No selection/detail panel is shown, or a clear "no outages" state — confirm no error occurs.

---

## Project Management

### OUT-58 — Create a project
**Priority:** Medium
**Steps:** Navigate to `/admin/projects`, enter TP Code and Project Name, optionally select a Zone, save.
**Expected Result:** `POST /api/projects` succeeds; project appears in the "Open" list; is selectable on Outage Create for non-Routine-Maintenance job types.

---

### OUT-59 — TP Code and Name required
**Steps:** Attempt to save with either field blank.
**Expected Result:** Blocked with "TP Code and Name are both required."

---

### OUT-60 — Mark project Complete with open outages warns
**Preconditions:** A project with `openOutageCount > 0`.
**Steps:** Click **Mark Complete**.
**Expected Result:** Confirmation dialog specifically warns "...still has N open outage(s)... Mark it Complete anyway?"; confirming proceeds, moving the project to the "Closed" list.

---

### OUT-61 — Mark project Complete with no open outages
**Steps:** Click **Mark Complete** on a project with zero open outages.
**Expected Result:** Generic confirm dialog (no open-outage warning); project moves to Closed on confirmation.

---

### OUT-62 — Re-open a closed project
**Steps:** On a Closed project, click **Re-open**, confirm.
**Expected Result:** `POST /projects/{id}/status` with `isActive=true` succeeds; project returns to the Open list and becomes selectable again on Outage Create.

---

## Outage Type Configuration

### OUT-63 — Create an outage type rule
**Priority:** High
**Steps:** Navigate to `/admin/outage-type-rules`, select Outage Type (Planned/Unplanned/Emergency/Forced), Work Type (Dead/Live), set a lead-time range (More Than / Less Than, in any combination of Days/Months/Years), select applicable Voltages (or leave empty for "ALL"), save.
**Expected Result:** `POST /api/outage-type-rules` succeeds; rule appears grouped under the correct Work Type section; `formatRange()` displays a human-readable summary (e.g. "More than 7d, less than 1mo 30d").

---

### OUT-64 — Edit an existing rule
**Steps:** Open an existing rule, change its range or voltages, save.
**Expected Result:** `PUT /api/outage-type-rules/{id}` succeeds; changes reflected immediately and affect subsequent Outage Create classification.

---

### OUT-65 — Deactivate a rule
**Steps:** Click **Remove** on a rule, confirm.
**Expected Result:** `DELETE /api/outage-type-rules/{id}` (soft-deactivate) succeeds; rule no longer appears in the active list and no longer participates in classification (cross-check OUT-09 for the resulting gap).

---

### OUT-66 — Overlapping rules — classification precedence
**Preconditions:** Two active rules for the same Work Type + Voltage with overlapping lead-time ranges.
**Steps:** Create both rules, then create an outage whose lead time falls within the overlap.
**Expected Result:** Confirm actual precedence behaviour (e.g. first match wins, most specific wins) — document actual system behaviour, since business rules likely expect non-overlapping ranges; flag as a defect if the result is inconsistent/unpredictable.

---

## Outage Scheduling

### OUT-67 — Toggle allowed months per Work Type/Outage Type
**Priority:** Medium
**Steps:** Navigate to `/admin/outage-scheduling`, toggle several cells in the grid (WorkType×OutageType rows vs. month columns), click **Save**.
**Expected Result:** `POST /api/outage-schedule-windows` succeeds with the full 96-entry grid; "Changes saved." message shown; reloading the page reflects the saved toggle states.

---

### OUT-68 — Scheduling window enforcement on Outage Create
**Preconditions:** A cell disabled (not allowed) for a specific WorkType+OutageType+Month combination.
**Steps:** Attempt to create/submit an outage whose classification and planned month match the disabled cell.
**Expected Result:** Confirm whether Outage Create actually enforces this restriction (code review did not confirm server-side enforcement on the Outages endpoint) — log as a defect/gap if outages can still be created for disallowed months.

---

## Authorisation Personnel

### OUT-69 — Create authorisation personnel
**Priority:** Medium
**Steps:** Navigate to `/admin/authorisation-personnel`, enter Full Name, Email, Zone (required), optionally Staff ID and Designation, save.
**Expected Result:** `POST /api/authorisation-personnel` succeeds; person appears in the list and becomes selectable as an Authoriser in GNC Take Active (script 09, GNC-02).

---

### OUT-70 — Required field validation
**Steps:** Attempt to save with Name, Email, or Zone blank.
**Expected Result:** Blocked with "Name, email, and zone are all required."

---

### OUT-71 — Edit and deactivate personnel
**Steps:** Edit an existing person's details and save; then deactivate ("Remove from list") a different person, confirming the dialog.
**Expected Result:** Edits persist; deactivated person no longer appears as a selectable Authoriser in GNC/other active-personnel dropdowns.

---

## Change Request Settings

### OUT-72 — Update the Change Request validity period
**Priority:** Low
**Steps:** Navigate to `/admin/change-request-settings`, change the Days value to a valid whole number ≥ 1, save.
**Expected Result:** Save succeeds with message "Changes apply immediately to all active users."

---

### OUT-73 — Reject invalid Days value
**Steps:** Enter 0, a negative number, or a non-integer value, attempt to save.
**Expected Result:** Blocked with "Enter a whole number of days, 1 or greater."

---

## Role / Access Control

### OUT-ROLE-01 — Workflow actions callable regardless of role (negative test)
**Priority:** High
**Preconditions:** Two test accounts of different roles, neither with explicit Planner/GNM authority.
**Steps:** Log in as a user without Planner/GNM authority; attempt Agree/Confirm/GNM-Approve actions (via UI where visible, and via direct navigation/API where the button may be hidden but the route/action is reachable).
**Expected Result (business expectation):** Only users with the correct role/authority should be able to perform Planner-agree, Requestor-confirm, and GNM-approve actions. **Actual behaviour per code review:** beyond the auto-status logic applied at outage creation (which checks PLANNER/GNM/GNM_ADMIN role codes), the review/approval action endpoints (`/agree`, `/confirm`, `/gnm-approve`, etc.) only require generic `[Authorize]` with no further role checks. **Log as a defect if confirmed** — this would allow any authenticated user to drive an outage through the entire approval chain.
