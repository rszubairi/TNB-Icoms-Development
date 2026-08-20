# UAT Script 12 — Single Line Diagrams (SLD)

**View(s):** SLD List (`/sld`), SLD Detail (`/sld/:id`)
**API:** `GET/POST /api/sld`, `GET /api/sld/{id}`, `POST /api/sld/{id}/drawing`, `GET /api/sld/{id}/drawing/download`, `PUT /api/sld/{id}/engineer-review`, `PUT /api/sld/{id}/se-review`, `PUT /api/sld/{id}/dce-review`, `PUT /api/sld/{id}/requestor-approve`

## Preconditions
- Logged in with roles covering Requestor, Engineer, S/E, DCE as needed per test.
- Zone, Station, Voltage Level reference data seeded.
- SLD status values (in order): `PendingEngineerReview` → `PendingSE` → `PendingDCE` → `PendingRequestorApproval` → `Published`, with `Rejected` reachable from the Engineer stage, and any stage rejection (after Engineer) routing back to `PendingEngineerReview` for rework.

---

## SLD List

### SLD-01 — List loads with status labels
**Priority:** High
**Steps:** Navigate to `/sld`.
**Expected Result:** `GET /api/sld` returns all diagrams; each row's raw status is rendered via the friendly label map (e.g. `PendingEngineerReview` → "Pending Engineer Review", `PendingSE` → "Pending S/E", `PendingDCE` → "Pending DCE", `PendingRequestorApproval` → "Pending Requestor Approval", `Published`, `Rejected"), not the raw enum string.

---

### SLD-02 — Filter by station/status (if exposed)
**Priority:** Medium
**Steps:** Apply a station or status filter on the list.
**Expected Result:** `GET /api/sld?stationId=&status=` returns only matching diagrams; clearing filters restores the full list.

---

### SLD-03 — Create SLD request — required fields
**Priority:** High
**Steps:** Click **+ New Diagram** (or equivalent); attempt Submit with Station or Voltage Level blank.
**Expected Result:** Blocked with "Station and voltage are required." Zone is pre-filled from the user's own zone and filters the Station dropdown (`stationsForZone`).

---

### SLD-04 — Create SLD request — successful submission
**Priority:** High
**Steps:** Select Zone, Station, Voltage Level, Flow Type (default "New" — verify other options e.g. Modification/Decommission if present), optional Remark; submit.
**Expected Result:** `POST /api/sld` succeeds; new diagram created with `Status = PendingEngineerReview`; appears in the list and is immediately navigable to its Detail page.

---

### SLD-05 — Flow Type variants
**Priority:** Medium
**Steps:** Create diagrams using each available Flow Type value (not just the default "New").
**Expected Result:** Each Flow Type is accepted and persisted correctly and is visible/distinguishable in the list or detail view.

---

## SLD Detail — Drawing Upload/Download

### SLD-06 — Detail page loads full record
**Priority:** High
**Steps:** Navigate to `/sld/:id`.
**Expected Result:** `GET /api/sld/{id}` returns diagram metadata, current status, mnemonic, substation type, and any uploaded drawing info; page renders without error.

---

### SLD-07 — Upload drawing file
**Priority:** High
**Steps:** Select a valid drawing file (e.g. PDF/image) via the file picker.
**Expected Result:** `POST /api/sld/{id}/drawing` (multipart, up to 25 MB per `RequestSizeLimit`) succeeds; page reloads showing the drawing is attached; "uploading" spinner state clears correctly.

---

### SLD-08 — Upload rejects empty/oversized file
**Priority:** Medium
**Steps:** Attempt to submit the upload with no file selected; separately, attempt a file larger than 25 MB.
**Expected Result:** No-file case: `onFileSelected` short-circuits, no request sent. Oversized case: server returns 413/400 (`RequestSizeLimit` exceeded); a clear error message is shown, not a raw failure.

---

### SLD-09 — Download drawing
**Priority:** Medium
**Preconditions:** A drawing has been uploaded (SLD-07).
**Steps:** Click **Download Drawing**.
**Expected Result:** `GET /api/sld/{id}/drawing/download` returns the file as `application/octet-stream`; browser triggers a save with a sensible filename derived from the diagram number.

---

### SLD-10 — Download with no drawing uploaded
**Priority:** Low
**Steps:** On a diagram with no uploaded drawing, attempt Download.
**Expected Result:** Server returns 404 with "No drawing has been uploaded for this diagram."; UI surfaces a clear message rather than a broken download.

---

## SLD Detail — Approval Workflow

### SLD-11 — Engineer Review — Approve requires Mnemonic
**Priority:** High
**Preconditions:** `Status = PendingEngineerReview`.
**Steps:** Attempt Engineer **Approve** with the Mnemonic field blank.
**Expected Result:** Blocked client-side with "Mnemonic is required." Substation Type defaults to "AIS" (verify other valid options, e.g. "GIS", are selectable).

---

### SLD-12 — Engineer Review — Approve success
**Priority:** High
**Steps:** Enter Mnemonic, select Substation Type, click Approve.
**Expected Result:** `PUT /api/sld/{id}/engineer-review` with `approve: true` succeeds; `Status` moves to `PendingSE`; Mnemonic and Substation Type are persisted on the diagram.

---

### SLD-13 — Engineer Review — Reject requires reason
**Priority:** High
**Steps:** Attempt Engineer **Reject** with Rejection Reason blank.
**Expected Result:** Blocked with "A rejection reason is required." — matches server contract where `approve: false` requires `rejectionReason`.

---

### SLD-14 — Engineer Review — Reject success
**Priority:** High
**Steps:** Enter a Rejection Reason, click Reject.
**Expected Result:** `PUT /api/sld/{id}/engineer-review` with `approve: false` succeeds; `Status = Rejected`; reason stored and visible on the diagram; diagram appears filterable as Rejected in the list.

---

### SLD-15 — S/E Review — Approve
**Priority:** High
**Preconditions:** `Status = PendingSE`.
**Steps:** Click S/E **Approve**.
**Expected Result:** `PUT /api/sld/{id}/se-review` with `approve: true` succeeds; `Status` moves to `PendingDCE`.

---

### SLD-16 — S/E Review — Adjust (send back) requires reason
**Priority:** High
**Steps:** Attempt S/E **Request Adjustment** with no reason entered; then with a reason.
**Expected Result:** Blocked with "A reason is required to request adjustment." when empty. With a reason, `PUT /api/sld/{id}/se-review` with `approve: false` succeeds and `Status` reverts to `PendingEngineerReview` for rework (per `SldService` logic setting status back to `PendingEngineerReview` on non-approve at this stage).

---

### SLD-17 — DCE Review — Approve
**Priority:** High
**Preconditions:** `Status = PendingDCE`.
**Steps:** Click DCE **Approve**.
**Expected Result:** `PUT /api/sld/{id}/dce-review` with `approve: true` succeeds; `Status` moves to `PendingRequestorApproval`.

---

### SLD-18 — DCE Review — Adjust requires reason
**Priority:** High
**Steps:** Attempt DCE **Request Adjustment** with no reason; then with a reason.
**Expected Result:** Same pattern as SLD-16 — blocked without reason; with reason, sends back to `PendingEngineerReview`.

---

### SLD-19 — Requestor Approve — final Publish
**Priority:** High
**Preconditions:** `Status = PendingRequestorApproval`.
**Steps:** Click Requestor **Approve**.
**Expected Result:** `PUT /api/sld/{id}/requestor-approve` with `approve: true` succeeds; `Status = Published` (terminal state); no further review actions are shown.

---

### SLD-20 — Requestor Adjust requires reason and routes back
**Priority:** High
**Steps:** Attempt Requestor **Request Adjustment** with no reason; then with a reason.
**Expected Result:** Blocked without reason; with reason, `PUT /api/sld/{id}/requestor-approve` with `approve: false` succeeds and `Status` reverts to `PendingEngineerReview`, restarting the full multi-stage chain (Engineer → S/E → DCE → Requestor).

---

## Role-Based Access & Edge Cases

### SLD-21 — Stage action hidden/blocked when out of turn
**Priority:** High
**Steps:** While `Status = PendingSE`, attempt to trigger the Engineer Review action directly (e.g. via API call bypassing the UI).
**Expected Result:** Server validates `sld.Status != expectedStatus` and returns "This diagram is not awaiting this stage's review (current status: {status})." — reject any out-of-sequence review call, not just hide the button.

---

### SLD-22 — Role restriction per review stage
**Priority:** High
**Steps:** As a user without the Engineer/S/E/DCE/Requestor role for the current stage, attempt the corresponding review action.
**Expected Result:** Action should be denied — either hidden client-side or rejected server-side with a clear message. **Action for UAT:** flag as a defect if `SldController` enforces no per-stage role/policy checks beyond `[Authorize]`, allowing any authenticated user to approve any stage.

---

### SLD-23 — Full multi-stage happy path end-to-end
**Priority:** High
**Steps:** Create a new diagram, then walk it through Engineer Approve → S/E Approve → DCE Approve → Requestor Approve in one continuous pass.
**Expected Result:** Diagram transitions cleanly through all five statuses (`PendingEngineerReview` → `PendingSE` → `PendingDCE` → `PendingRequestorApproval` → `Published`) with no stuck or skipped stages; each stage's approver/timestamp is recorded.
