# UAT Script 13 — Commissioning Memos

**View(s):** Memo List (`/commissioning-memos`), Memo Detail (`/commissioning-memos/:id`)
**API:** `CommissioningMemosController.cs` (`api/commissioning-memos`)

## Preconditions
- Logged in as a user in the TOMS/GNM approval chain. At least one existing Outage record to reference.
- **Known gap:** the backend enforces workflow stage transitions only by record `Status`, not by the caller's role — no role/policy checks found on `CommissioningMemosController`. Treat CM-ROLE-* below as a high-priority defect to confirm.

---

### CM-01 — Memo list loads
**Priority:** High
**Steps:** Navigate to `/commissioning-memos`.
**Expected Result:** Table shows Memo No., Outage, Type, Status (badge), Result, Submitted By, Submitted At.

---

### CM-02 — Create a new memo — look up outage
**Steps:**
1. Click **+ New Memo**.
2. Enter an Outage ID and click **Look Up**.

**Expected Result:** Outage number, station, and description are displayed if found; "Outage not found." message shown for an invalid/non-existent ID.

---

### CM-03 — Create memo — Commissioning / Decommissioning type
**Steps:**
1. Look up a valid Outage.
2. Select Memo Type = Commissioning (or Decommissioning).
3. Enter Switching Program.
4. Enter Data Form.
5. Check applicable Commissioning Requirement Documents (IOM Endorsed, MTEP Protection Letter, Resident Engineer Certification, Form G, Form H, Metering Email Chain, SCADA Email Chain, HGSO Letter).
6. Click **Submit**.

**Expected Result:** `POST api/commissioning-memos` succeeds; new memo created with status `PendingEngineerPic`; appears in the list.

---

### CM-04 — Create memo — Emergency Commissioning type hides Data Form/checklist
**Steps:** In the New Memo form, select Memo Type = "Emergency Commissioning (Switching Program only)".
**Expected Result:** Data Form field and the 8-item Requirement Documents checklist are hidden/not required; only Outage lookup and Switching Program are needed to submit.

---

### CM-05 — Required field validation
**Steps:** Attempt to submit with Outage not looked up, or Switching Program blank, or (for non-emergency types) Data Form blank.
**Expected Result:** Submission blocked with inline validation errors for each missing required field.

---

### CM-06 — Cancel new memo form
**Steps:** Open the New Memo form, enter data, click **Cancel**.
**Expected Result:** Form closes without creating a record.

---

### CM-07 — Memo detail — timeline and checklist view
**Steps:** Open an existing memo from the list.
**Expected Result:** Detail shows Memo No., Status badge, Memo Type, Outage number, Switching Program, Data Form (if present), the 8-item checklist (hidden for Emergency type), and a timeline of Engineer PIC / S/E / DCE / CE GNM / Final Sign-off with approver name + timestamp once reached.

---

### CM-08 — Download Cover Page PDF
**Steps:** From the memo detail page, click **Download Cover Page PDF**.
**Expected Result:** `GET api/commissioning-memos/{id}/cover-page.pdf` returns a correctly formatted, server-generated PDF reflecting the memo's current data.

---

## Stage 1 — Engineer PIC (status = PendingEngineerPic)

### CM-09 — Engineer PIC approve
**Steps:** On a `PendingEngineerPic` memo, click **Approve → S/E**.
**Expected Result:** Status becomes `PendingSE`.

---

### CM-10 — Engineer PIC reject
**Steps:** On a `PendingEngineerPic` memo, enter a Rejection Reason, click **Reject**.
**Expected Result:** Status remains/returns to `PendingEngineerPic` (there is no terminal Rejected state in this workflow — every rejection loops back to this stage); rejection reason is shown on the detail page.

---

### CM-11 — Reject requires a reason
**Steps:** Attempt **Reject** with the Rejection Reason field empty.
**Expected Result:** Rejection blocked until a reason is provided.

---

## Stage 2 — S/E (status = PendingSE)

### CM-12 — S/E approve
**Steps:** On a `PendingSE` memo, click **Approve → DCE**.
**Expected Result:** Status becomes `PendingDCE`.

---

### CM-13 — S/E reject loops back to Engineer PIC
**Steps:** On a `PendingSE` memo, enter a Rejection Reason, click **Reject**.
**Expected Result:** Status returns to `PendingEngineerPic`; reason visible on detail.

---

## Stage 3 — DCE (status = PendingDCE)

### CM-14 — DCE approve
**Steps:** On a `PendingDCE` memo, click **Approve → CE GNM**.
**Expected Result:** Status becomes `PendingCeGnm`.

---

### CM-15 — DCE reject loops back to Engineer PIC
**Steps:** On a `PendingDCE` memo, reject with a reason.
**Expected Result:** Status returns to `PendingEngineerPic`.

---

## Stage 4 — CE GNM (status = PendingCeGnm)

### CM-16 — CE GNM approve
**Steps:** On a `PendingCeGnm` memo, click **Approve → Final Sign-off**.
**Expected Result:** Status becomes `PendingFinalSignOff`.

---

### CM-17 — CE GNM reject loops back to Engineer PIC
**Steps:** On a `PendingCeGnm` memo, reject with a reason.
**Expected Result:** Status returns to `PendingEngineerPic`.

---

## Stage 5 — Final Sign-off (status = PendingFinalSignOff)

### CM-18 — Final approve
**Steps:** On a `PendingFinalSignOff` memo, click **Final Approve**.
**Expected Result:** Status becomes `Approved` (terminal for the approval workflow); success message shown; a Commissioning Result selector appears.

---

### CM-19 — Final reject loops back to Engineer PIC
**Steps:** On a `PendingFinalSignOff` memo, reject with a reason.
**Expected Result:** Status returns to `PendingEngineerPic` — the memo must go through the entire chain again.

---

## Post-Approval — Commissioning Result

### CM-20 — Record commissioning result
**Steps:** On an `Approved` memo, select a Commissioning Result (In Progress / On Soak / Comm Successful / Comm Not Successful), click **Record Result**.
**Expected Result:** `PUT api/commissioning-memos/{id}/commissioning-result` succeeds; result is saved and displayed in both the detail page and the list's Result column. This does not change the approval `Status`.

---

### CM-21 — Change result after initial recording
**Steps:** On an already-resulted `Approved` memo, change the Commissioning Result to a different value, save.
**Expected Result:** Confirm whether the business allows updating a previously recorded result — verify actual behaviour (update succeeds, or is blocked) against requirements.

---

### CM-22 — Filter memo list by Outage / Status
**Steps:** Apply Outage and/or Status filters (`GET api/commissioning-memos?outageId=&status=`) if exposed in the UI.
**Expected Result:** List narrows correctly to matching records.

---

### CM-ROLE-01 — Role enforcement gap (negative test)
**Priority:** High
**Preconditions:** Two test accounts of different roles.
**Steps:** Log in as a user who should NOT be authorized to perform the Engineer PIC/S/E/DCE/CE GNM/Final Sign-off action for a given memo stage; open a memo in that stage; attempt the approve/reject action.
**Expected Result (business expectation):** Action should be blocked for users without the correct authority for that stage. **Actual behaviour per code review:** only generic `[Authorize]` is enforced (any authenticated user) — action will likely succeed regardless of role. **Log as a defect if confirmed.**
