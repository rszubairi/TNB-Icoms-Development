# UAT Script 06 — Organisations & Stations

**View(s):** `/admin/organisations`
**API:** `OrganisationsController.cs`, `StationsController.cs`

## Preconditions
- Logged in as Admin/System Setup user.

---

### ORG-01 — Organisations list loads
**Priority:** High
**Steps:** Navigate to `/admin/organisations`.
**Expected Result:** List of organisations displayed with key attributes (name, type/zone, associated stations count, status).

---

### ORG-02 — Create a new organisation
**Steps:** Click **Add Organisation**, complete required fields (name, zone/region, contact details as applicable), save.
**Expected Result:** New organisation created and appears in the list.

---

### ORG-03 — Required field validation
**Steps:** Attempt to save with mandatory fields blank.
**Expected Result:** Save blocked, inline validation errors shown.

---

### ORG-04 — Edit an organisation
**Steps:** Open an existing organisation, update details, save.
**Expected Result:** Changes persist and are reflected in the list.

---

### ORG-05 — Deactivate an organisation
**Steps:** Deactivate an organisation with no active dependencies.
**Expected Result:** Organisation is marked Inactive; no longer selectable in dependent dropdowns (e.g. User creation Organisation field).

---

### ORG-06 — Add a station under an organisation
**Steps:** From an organisation's detail view (or a dedicated Stations screen), add a new station with name, zone, voltage level, and coordinates/location as applicable.
**Expected Result:** Station is created and linked to the correct organisation; appears in downstream dropdowns (e.g. Equipment Directory, GCU Stations on User form).

---

### ORG-07 — Edit a station
**Steps:** Open an existing station, update its details, save.
**Expected Result:** Changes persist and propagate to dependent screens (e.g. Outage creation station selector).

---

### ORG-08 — Prevent deletion of station/organisation in use
**Steps:** Attempt to delete an organisation or station that has associated equipment, outages, or users.
**Expected Result:** Deletion blocked with a clear message, or a soft-delete/deactivate path is offered instead.

---

### ORG-09 — Duplicate organisation/station name rejected
**Steps:** Attempt to create an organisation or station with a name that already exists within the same zone.
**Expected Result:** Save fails with a clear duplicate error.

---

### ORG-10 — Non-admin access restriction
**Preconditions:** Login as non-admin role.
**Steps:** Attempt to navigate to `/admin/organisations` directly.
**Expected Result:** Access denied/redirected, or read-only depending on business rules.
