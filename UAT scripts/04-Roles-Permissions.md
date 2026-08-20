# UAT Script 04 — Roles & Permissions

**View(s):** `/admin/roles` — `features/admin/users/roles/`
**API:** `RolesController.cs` (`GET/POST/PUT/DELETE /api/roles`)

## Preconditions
- Logged in as Admin/System Setup user.

---

### ROLE-01 — Role list loads
**Priority:** High
**Steps:** Navigate to `/admin/roles`.
**Expected Result:** List of existing roles displayed (e.g. Admin, Requestor/Planner, TOMS/GNM, GNC, GCU), each with associated permission summary.

---

### ROLE-02 — Create new role
**Steps:** Click **Add Role** (or equivalent), enter a role name/description, select applicable permissions, save.
**Expected Result:** New role is created and appears in the list; `POST /api/roles` succeeds.

---

### ROLE-03 — Required field validation on role creation
**Steps:** Attempt to save a new role with the name field blank.
**Expected Result:** Form blocks submission; inline validation error shown.

---

### ROLE-04 — Duplicate role name rejected
**Steps:** Attempt to create a role with a name that already exists.
**Expected Result:** Save fails with a clear error message.

---

### ROLE-05 — Edit role permissions
**Steps:** Open an existing role, toggle one or more permission checkboxes, save.
**Expected Result:** `PUT /api/roles/{id}` succeeds; changes persist on reopening the role.

---

### ROLE-06 — Permission change affects existing users
**Steps:** Remove a permission from a role that has users assigned to it; log in (or have that user log in) and verify affected functionality is no longer accessible.
**Expected Result:** User assigned to the modified role immediately (or after re-login/token refresh) loses access to the removed capability.

---

### ROLE-07 — Delete/deactivate a role not in use
**Steps:** Attempt to delete a role with no users assigned.
**Expected Result:** Role is removed/deactivated successfully.

---

### ROLE-08 — Prevent deletion of a role currently assigned to users
**Steps:** Attempt to delete a role that has one or more users assigned.
**Expected Result:** Deletion is blocked with a clear message (e.g. "Role is in use"), or user is prompted to reassign affected users first.

---

### ROLE-09 — Non-admin access restriction
**Preconditions:** Login as non-admin role.
**Steps:** Attempt to navigate to `/admin/roles` directly.
**Expected Result:** Access denied/redirected.
