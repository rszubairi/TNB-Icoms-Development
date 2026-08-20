# UAT Script 05 — Role Transfer Requests

**View(s):** `/admin/role-transfer-requests`
**API:** `RoleTransferRequestsController.cs`

## Preconditions
- Logged in as Admin/System Setup user (approver). A second test user account to act as requestor, if self-service submission is supported.

---

### RTR-01 — Role transfer request list loads
**Priority:** High
**Steps:** Navigate to `/admin/role-transfer-requests`.
**Expected Result:** List of pending/historical role transfer requests displayed, with requestor, current role, requested role, status, and date columns.

---

### RTR-02 — Submit a new role transfer request
**Preconditions:** Feature accessible to the requesting user (e.g. via Account page or this admin page, depending on implementation — confirm actual entry point during testing).
**Steps:** Initiate a role transfer request specifying target role and reason.
**Expected Result:** Request is created with status "Pending" and appears in the list.

---

### RTR-03 — Approve a pending request
**Steps:** Open a Pending request, click **Approve**.
**Expected Result:** Request status changes to "Approved"; the affected user's role is updated accordingly (cross-check via User Management, USR-14). An email/notification is triggered if applicable (cross-check Email Logs, script 14).

---

### RTR-04 — Reject a pending request
**Steps:** Open a Pending request, click **Reject**, optionally provide a reason.
**Expected Result:** Request status changes to "Rejected"; the affected user's role remains unchanged.

---

### RTR-05 — Reason/comment required on rejection
**Steps:** Attempt to reject without entering a reason (if the UI requires one).
**Expected Result:** Rejection is blocked until a reason is provided, if the business rule requires it — confirm actual behaviour and log as informational if not enforced.

---

### RTR-06 — Cannot re-approve/re-reject an already-actioned request
**Steps:** Open a request already marked Approved or Rejected.
**Expected Result:** Approve/Reject actions are disabled or hidden; request is shown as read-only/historical.

---

### RTR-07 — Filter by status
**Steps:** Use any status filter (Pending/Approved/Rejected) if present.
**Expected Result:** List correctly filters to the selected status.

---

### RTR-08 — Non-admin access restriction
**Preconditions:** Login as non-admin role.
**Steps:** Attempt to navigate to `/admin/role-transfer-requests` directly.
**Expected Result:** Approval actions are not accessible to non-admin roles; access is denied or the page is read-only/hidden as per business rules.
