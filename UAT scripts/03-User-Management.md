# UAT Script 03 — User Management

**View(s):** User List (`/admin/users`), User Create/Edit (`/admin/users/new`, `/admin/users/:id`)
**API:** `GET /api/users`, `GET /api/users/{id}`, `POST /api/users`, `PUT /api/users/{id}`, `DELETE /api/users/{id}`

## Preconditions
- Logged in as an Admin/System Setup user.
- At least one existing user record, plus Role and Zone reference data seeded.

---

## User List

### USR-01 — User list loads with expected columns
**Steps:** Navigate to `/admin/users`.
**Expected Result:** Table displays TNB ID, Name, Email, Role (badge), Zone (badge), Status (Active/Inactive badge), and an Edit action per row.

---

### USR-02 — Search filter
**Steps:** Enter a partial name, TNB ID, or email into the search box.
**Expected Result:** Table filters client-side to matching rows only; clearing the search restores the full list.

---

### USR-03 — Filter by Role
**Steps:** Select a Role from the Role dropdown filter.
**Expected Result:** Only users with that role are shown. "All roles" (or equivalent) resets the filter.

---

### USR-04 — Filter by Zone
**Steps:** Select a Zone from the Zone dropdown filter.
**Expected Result:** Only users in that zone are shown.

---

### USR-05 — Combined filters
**Steps:** Apply search + Role + Zone filters together.
**Expected Result:** Table shows only users matching all active filter criteria simultaneously.

---

### USR-06 — Navigate to Add User
**Steps:** Click **+ Add User**.
**Expected Result:** Navigates to `/admin/users/new` with a blank form.

---

### USR-07 — Navigate to Edit User
**Steps:** Click **Edit** on an existing user row.
**Expected Result:** Navigates to `/admin/users/:id`, form pre-populated with that user's data (`GET /api/users/{id}`).

---

## User Create

### USR-08 — Create user with all required fields
**Steps:**
1. Navigate to `/admin/users/new`.
2. Enter TNB ID, Full Name, Email, Phone Number.
3. Select Role and Zone.
4. Optionally set Organisation, GCU Type, GCU Stations.
5. Click **Save**.

**Expected Result:** `POST /api/users` succeeds; user is created and appears in the User List with correct data; user is redirected back to the list (or shown a success message).

---

### USR-09 — Required field validation
**Steps:** On the create form, leave TNB ID, Full Name, Email, Phone Number, Role, or Zone blank one at a time; attempt to click **Save**.
**Expected Result:** Form is not submitted; inline validation errors appear for each missing required field after `markAllAsTouched`.

---

### USR-10 — Email format validation
**Steps:** Enter an invalid email (e.g. `notanemail`) into the Email field.
**Expected Result:** Inline validation error indicates invalid email format; form cannot be saved until corrected.

---

### USR-11 — Duplicate TNB ID / Email rejected
**Steps:** Attempt to create a user with a TNB ID or Email that already exists.
**Expected Result:** Save fails; a clear server-side error message is displayed (not a generic 500 error).

---

### USR-12 — GCU Stations multi-select
**Steps:** On the create form, select one or more GCU Stations checkboxes.
**Expected Result:** Selections are saved and reflected when reopening the user for edit. **Action for UAT:** verify the station list shown reflects real/current stations, not the hardcoded placeholder set (Station A/B/C/D) found in code review — flag as defect if placeholder data is present in a production-facing build.

---

### USR-13 — Is Active checkbox on create
**Steps:** Uncheck "Is Active" while creating a new user, save.
**Expected Result:** New user is created with Inactive status, correctly reflected in the User List badge.

---

## User Edit

### USR-14 — Edit and save changes
**Steps:** Open an existing user, change Full Name/Email/Phone/Role/Zone, click **Save**.
**Expected Result:** `PUT /api/users/{id}` succeeds; changes are reflected in the User List and when reopening the record.

---

### USR-15 — TNB ID is read-only on edit
**Steps:** Open an existing user for edit; attempt to change the TNB ID field.
**Expected Result:** Field is disabled/read-only and cannot be modified.

---

### USR-16 — Deactivate user
**Steps:** Open an existing active user, uncheck "Is Active", save. Alternatively, use a dedicated deactivate action if present.
**Expected Result:** `DELETE /api/users/{id}` (soft delete) or equivalent update sets the user to Inactive; user shows as Inactive in the list; user can no longer log in (cross-check with LOGIN-01 using their credentials).

---

### USR-17 — Reactivate a deactivated user
**Steps:** Open an Inactive user, re-check "Is Active", save.
**Expected Result:** User status returns to Active in the list and the user can log in again.

---

### USR-18 — Cancel without saving
**Steps:** Open a user for edit, make a change, then navigate away without clicking Save (e.g. back link).
**Expected Result:** No changes are persisted; reopening the record shows original data.

---

### USR-19 — Non-admin access restriction
**Preconditions:** Login as a non-admin role.
**Steps:** Attempt to access `/admin/users` and `/admin/users/new` directly by URL.
**Expected Result:** Access should be denied/redirected. **Action for UAT:** flag as defect if the page loads and functions for non-admin roles — code review found no explicit role/policy attribute on `UsersController`.
