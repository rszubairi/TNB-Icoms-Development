# UAT Script 15 — Account (Self-Service)

**View(s):** Account (`/account`)
**API:** `AccountController.cs` (`api/account/me`), `RoleTransferRequestsController.cs`, `RoleService.cs`, `ZoneService.cs`

## Preconditions
- Logged in as any authenticated user.

---

## Profile

### ACC-01 — Profile loads
**Priority:** High
**Steps:** Navigate to `/account`.
**Expected Result:** `GET /api/account/me` succeeds; Full Name, Email, Phone Number, and read-only details (TNB ID, Role, Zone) are displayed.

---

### ACC-02 — Edit profile — successful save
**Steps:** Click **Edit**, change Full Name, Email, and/or Phone Number, click **Save**.
**Expected Result:** `PUT /api/account/me` succeeds; updated values are reflected immediately on the page and persist on reload.

---

### ACC-03 — Edit profile — required field validation
**Steps:** Clear Full Name or Email, attempt to save.
**Expected Result:** Blocked with "Name and email are required."; Phone Number remains optional.

---

### ACC-04 — Cancel profile edit
**Steps:** Click **Edit**, change a field, click **Cancel**.
**Expected Result:** Form closes without saving; original values are restored on screen.

---

### ACC-05 — Duplicate email rejected
**Preconditions:** Another user already exists with a given email.
**Steps:** Attempt to change your email to one already in use by another account.
**Expected Result:** Save fails with a clear server-side error message.

---

## Role/Zone Transfer Request

### ACC-06 — Submit a transfer request — role only
**Steps:** Click the role/zone change action, select a Requested Role (leave Zone unset), enter a Reason, submit.
**Expected Result:** `POST /api/role-transfer-requests` succeeds; confirmation message shown; request appears in Admin's Role Transfer Requests list (script 05) as Pending.

---

### ACC-07 — Submit a transfer request — zone only
**Steps:** Select a Requested Zone (leave Role unset), enter a Reason, submit.
**Expected Result:** Request created successfully with only the zone change requested.

---

### ACC-08 — Submit a transfer request — both role and zone
**Steps:** Select both a Requested Role and Requested Zone, enter a Reason, submit.
**Expected Result:** Request created successfully with both changes requested.

---

### ACC-09 — Transfer request requires at least one target
**Steps:** Leave both Requested Role and Requested Zone unset, enter a Reason, attempt to submit.
**Expected Result:** Blocked with "Select a new role, a new zone, or both."

---

### ACC-10 — Transfer request requires a reason
**Steps:** Select a Requested Role, leave the Reason blank, attempt to submit.
**Expected Result:** Blocked with "A request summary is required."

---

### ACC-11 — Cancel transfer request form
**Steps:** Open the transfer request form, enter data, close/cancel without submitting.
**Expected Result:** No request is created.

---

## Password Change

### ACC-12 — Change password — successful
**Steps:** Open the password change form, enter correct Current Password, a New Password (≥8 characters), matching Confirm Password, submit.
**Expected Result:** `POST /api/account/me/password` succeeds; confirmation shown; user can log out and log back in with the new password (cross-check LOGIN-01/LOGIN-02).

---

### ACC-13 — Change password — all fields required
**Steps:** Leave any of Current Password, New Password, or Confirm Password blank, attempt to submit.
**Expected Result:** Blocked with "All fields are required."

---

### ACC-14 — Change password — mismatch rejected
**Steps:** Enter a New Password and a different Confirm Password, submit.
**Expected Result:** Blocked with "New password and confirmation do not match."

---

### ACC-15 — Change password — minimum length enforced
**Steps:** Enter a New Password shorter than 8 characters, submit.
**Expected Result:** Blocked with "New password must be at least 8 characters."

---

### ACC-16 — Change password — incorrect current password rejected
**Steps:** Enter an incorrect Current Password, a valid New Password and Confirm Password, submit.
**Expected Result:** Server rejects the request with a clear error; password is not changed.

---

### ACC-17 — Cancel password change form
**Steps:** Open the password form, enter data, close/cancel without submitting.
**Expected Result:** Password remains unchanged; user can still log in with the original password.
