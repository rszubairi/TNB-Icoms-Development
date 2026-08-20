# UAT Script 11 — Shift Handover

**View(s):** Shift Handover (`/handover`)
**API:** `GET /api/handover/categories`, `GET /api/handover/shift`, `GET /api/handover/shifts`, `PUT /api/handover/shifts/{id}/control`, `POST /api/handover/shifts/{id}/entries`, `DELETE /api/handover/entries/{id}`, `POST /api/handover/shifts/{id}/pass`

## Preconditions
- Logged in as a user with a Zone assigned (`authService.currentUser().zoneId`); users without a zone cannot use this page (see HO-01).
- Authorisation Personnel reference data seeded and active, for the Control Team roles.
- Three shift types exist: `Morning`, `Evening`, `Night` — a 3-shift logbook per day per zone.

---

## Shift Load & Navigation

### HO-01 — No-zone user blocked
**Priority:** High
**Preconditions:** Logged in as a user with no `zoneId` on their account.
**Steps:** Navigate to `/handover`.
**Expected Result:** Page shows "No zone assigned to your account." and does not attempt to load a shift.

---

### HO-02 — Get-or-create shift for today
**Priority:** High
**Steps:** Navigate to `/handover` as a user with a zone assigned.
**Expected Result:** `GET /api/handover/shift?shiftDate=&shiftType=Morning&zoneId=` returns (or creates) today's Morning shift for the user's zone; Control Team fields and entry categories render.

---

### HO-03 — Switch shift date
**Priority:** Medium
**Steps:** Change the Shift Date picker to a different day.
**Expected Result:** Shift reloads for the new date (same zone, same shift type); a new shift is created via get-or-create if none exists yet for that date.

---

### HO-04 — Switch shift type (Morning / Evening / Night)
**Priority:** High
**Steps:** Toggle between Morning, Evening, and Night for the same date.
**Expected Result:** Each shift type loads its own independent shift record — control team, entries, and pass/lock state are all per shift-type, confirming the 3-shift logbook model.

---

### HO-05 — Category tabs load from server
**Priority:** Medium
**Steps:** Observe the category tab list on page load.
**Expected Result:** Categories are fetched from `GET /api/handover/categories` (backed by `HandoverCategories.All`), not hardcoded in the component; the first category is auto-selected as active.

---

## Control Team

### HO-06 — Set Control Team for the shift
**Priority:** High
**Steps:** Select Control Manager, Switch Engineer 1, Switch Engineer 2, Despatcher, and Control Assistant from the (active) Authorisation Personnel dropdowns; click **Save**.
**Expected Result:** `PUT /api/handover/shifts/{id}/control` succeeds; values persist and are pre-filled correctly when reloading or switching away and back to this shift.

---

### HO-07 — Control Team fields are optional individually
**Priority:** Low
**Steps:** Save the Control Team with only some roles filled (e.g. only Control Manager).
**Expected Result:** Save succeeds; unfilled roles remain null and do not block saving — verify whether any role should actually be mandatory before shift Pass (cross-check with HO-11).

---

### HO-08 — Control Team edit after entries added
**Priority:** Low
**Steps:** Add a few log entries, then return and change a Control Team member, save.
**Expected Result:** Control Team update does not affect existing entries; both persist independently.

---

## Log Entries

### HO-09 — Add entry to active category
**Priority:** High
**Steps:** Select a category tab, enter a Description, optionally link a Related Outage ID, click **Add Entry**.
**Expected Result:** `POST /api/handover/shifts/{id}/entries` succeeds with the entry's `category` set to the currently active tab; entry appears immediately in that category's list; description field clears after add.

---

### HO-10 — Add entry blocked when description empty
**Priority:** Medium
**Steps:** Leave Description blank; click **Add Entry**.
**Expected Result:** No request is sent (`addEntry` short-circuits on empty/whitespace description); no blank entry is created.

---

### HO-11 — Entries filtered per category tab
**Priority:** Medium
**Steps:** Add entries under two different category tabs; switch between tabs.
**Expected Result:** `entriesForActiveCategory` shows only entries whose `category` matches the currently selected tab — entries from other categories are hidden, not deleted.

---

### HO-12 — Remove entry
**Priority:** Medium
**Steps:** Click **Remove** on an existing entry; confirm the "Remove this entry?" dialog.
**Expected Result:** `DELETE /api/handover/entries/{id}` succeeds; entry disappears from the list. Cancelling the dialog leaves the entry intact.

---

### HO-13 — Entry linked to a Related Outage
**Priority:** Low
**Steps:** Add an entry with a Related Outage ID pointing to an existing outage.
**Expected Result:** Entry saves with `relatedOutageId` set; UI shows a reference/link back to that outage (verify actual link behaviour on the detail view — flag if the link is missing or broken).

---

## Pass Handover (Shift Lock)

### HO-14 — Pass handover locks the shift
**Priority:** High
**Steps:** Click **Pass Handover**; confirm "Pass this {shiftType} shift's handover to the next shift? This locks it."
**Expected Result:** `POST /api/handover/shifts/{id}/pass` succeeds; shift becomes locked/read-only (verify entries and Control Team can no longer be edited on a passed shift — flag as a defect if editing is still possible after Pass).

---

### HO-15 — Cannot edit a passed (locked) shift
**Priority:** High
**Preconditions:** A shift has already been passed (HO-14).
**Steps:** Reopen that shift (via date/type or History) and attempt to add an entry or change Control Team.
**Expected Result:** Add Entry / Save Control Team controls are disabled, or the server rejects the write with a clear "shift already passed" error — never a silent no-op or unhandled exception.

---

### HO-16 — Cancel Pass Handover dialog
**Priority:** Low
**Steps:** Click **Pass Handover**, then cancel the browser confirm dialog.
**Expected Result:** No request sent; shift remains unlocked and editable.

---

## Shift History

### HO-17 — Toggle and load shift history
**Priority:** Medium
**Steps:** Click **History** (or equivalent toggle).
**Expected Result:** `GET /api/handover/shifts?zoneId=` loads a summary list of past shifts for the zone; toggling again hides the panel without re-fetching every time.

---

### HO-18 — Open a shift from history
**Priority:** Medium
**Steps:** From the History panel, click a past shift entry.
**Expected Result:** Shift Date and Shift Type update to match the selected history row; the corresponding shift loads (read-only if passed, per HO-15); History panel closes.

---

## Role-Based Access & Edge Cases

### HO-19 — Cross-zone isolation
**Priority:** High
**Steps:** As a user in Zone A, verify no way exists (via URL manipulation or otherwise) to view or edit Zone B's handover shift.
**Expected Result:** All handover endpoints are scoped by the caller's own `zoneId`; attempting to pass a different zone's ID (if the API allows a zoneId query param override) should be rejected or ignored server-side. **Action for UAT:** flag as a defect if `zoneId` is trusted from client input without server-side validation against the authenticated user's assigned zone.

---

### HO-20 — Concurrent editors on the same shift
**Priority:** Low
**Steps:** Open the same shift in two sessions; add an entry in one, then add a different entry in the other without reloading first.
**Expected Result:** Both entries are eventually visible after reload; no data loss or overwrite of the Control Team fields between the two sessions (verify there is no last-write-wins clobbering of unrelated fields).
