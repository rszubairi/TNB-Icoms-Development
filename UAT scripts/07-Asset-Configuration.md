# UAT Script 07 — Asset Configuration

**View(s):** Voltage & Equipment Types (`/admin/voltage-equipment`), Equipment Directory (`/admin/equipment`), Off-Point Management (`/admin/off-points`), Dropdown Management (`/admin/dropdown-values`), Transmission Lines (`/admin/transmission-lines`), Conflicting Lines (`/admin/conflicting-lines`), Linking Lines (`/admin/linking-lines`), Mnemonic List (`/admin/mnemonic`)
**API:** `VoltageLevelsController.cs`, `EquipmentTypesController.cs`, `EquipmentController.cs`, `DropdownValuesController.cs`, `TransmissionLinesController.cs`, `ConflictingLinesController.cs`, `LinkingLinesController.cs`, `MnemonicController.cs`

## Preconditions
- Logged in as Admin/System Setup user.
- Stations and zones already configured (see script 06).

---

## Voltage & Equipment Types

### CFG-01 — Voltage levels list loads and CRUD works
**Priority:** High
**Steps:** Navigate to `/admin/voltage-equipment`. Add a new voltage level (e.g. "132kV"), edit an existing one, then deactivate one.
**Expected Result:** Each action succeeds and is reflected in the list immediately; deactivated voltage levels no longer appear in dependent dropdowns (e.g. Equipment Directory, Outage creation).

---

### CFG-02 — Equipment types CRUD
**Steps:** In the same view (or its equipment-types tab/section), add, edit, and deactivate an Equipment Type (e.g. "Circuit Breaker", "Transformer").
**Expected Result:** CRUD operations succeed; changes reflected in Equipment Directory's Equipment Type selector.

---

### CFG-03 — Duplicate voltage level / equipment type rejected
**Steps:** Attempt to create a voltage level or equipment type with a name that already exists.
**Expected Result:** Save fails with a clear duplicate error.

---

## Equipment Directory

### CFG-04 — Equipment list loads with filters
**Steps:** Navigate to `/admin/equipment`. Filter by Station, Voltage Level, and/or Equipment Type.
**Expected Result:** List loads all configured equipment; filters correctly narrow results.

---

### CFG-05 — Create new equipment
**Steps:** Click **Add Equipment**, complete required fields (name/tag, Station, Voltage Level, Equipment Type, status), save.
**Expected Result:** New equipment record created and appears in the directory; selectable in downstream screens (e.g. Outage creation equipment picker).

---

### CFG-06 — Required field validation on equipment creation
**Steps:** Attempt to save with mandatory fields blank.
**Expected Result:** Save blocked, inline validation errors shown.

---

### CFG-07 — Edit equipment
**Steps:** Open an existing equipment record, update its details, save.
**Expected Result:** Changes persist and propagate correctly.

---

### CFG-08 — Deactivate equipment in use
**Steps:** Attempt to deactivate equipment currently referenced by an active/pending outage.
**Expected Result:** Either blocked with a warning, or deactivation is allowed but existing outage records retain the historical reference — confirm actual behaviour against business rules.

---

## Off-Point Management

### CFG-09 — Off-point list and CRUD
**Steps:** Navigate to `/admin/off-points`. Add, edit, and remove an off-point record (linked to equipment/station).
**Expected Result:** CRUD succeeds; off-points are correctly selectable when creating an outage requiring off-point declarations.

---

## Dropdown Management

### CFG-10 — Dropdown values list by category
**Steps:** Navigate to `/admin/dropdown-values`. Select a dropdown category (e.g. Outage Reason, Priority).
**Expected Result:** Values for the selected category are listed.

---

### CFG-11 — Add/edit/reorder dropdown values
**Steps:** Add a new value to a category, edit an existing value's label, and reorder if drag/reorder is supported.
**Expected Result:** Changes save successfully and are immediately reflected in the corresponding dropdown wherever it's used elsewhere in the app (e.g. Outage creation reason field).

---

### CFG-12 — Deactivate a dropdown value in use
**Steps:** Deactivate a value that is currently selected on an existing record.
**Expected Result:** Value no longer appears as a selectable option for new records, but historical records retain the original value/label.

---

## Transmission Lines / Conflicting Lines / Linking Lines

### CFG-13 — Transmission line CRUD
**Steps:** Navigate to `/admin/transmission-lines`. Add a new transmission line (name, connected stations, voltage level), edit, and deactivate.
**Expected Result:** CRUD operations succeed; line appears in downstream selectors (Outage creation, Conflicting/Linking Lines config).

---

### CFG-14 — Conflicting lines configuration
**Steps:** Navigate to `/admin/conflicting-lines`. Define a conflict relationship between two transmission lines, save.
**Expected Result:** Relationship saved; when creating/scheduling an outage on one of the conflicting lines, the system should surface a conflict warning (cross-check with script 08, Outage Scheduling).

---

### CFG-15 — Prevent self-referencing conflict
**Steps:** Attempt to set a transmission line as conflicting with itself.
**Expected Result:** Save is blocked with a validation error.

---

### CFG-16 — Linking lines configuration
**Steps:** Navigate to `/admin/linking-lines`. Define a link relationship between two lines/stations, save.
**Expected Result:** Relationship is saved and correctly referenced wherever linking-line logic applies (e.g. outage impact assessment).

---

## Mnemonic List

### CFG-17 — Mnemonic list view
**Steps:** Navigate to `/admin/mnemonic`.
**Expected Result:** List of existing mnemonics/codes displayed.

---

### CFG-18 — Upload mnemonic list (bulk import)
**Steps:** Click the upload/import action, select a valid mnemonic file (per accepted format — CSV/Excel), submit.
**Expected Result:** File is parsed and mnemonics are imported; success message with count of records imported is shown; new/updated mnemonics appear in the list.

---

### CFG-19 — Upload invalid file rejected
**Steps:** Attempt to upload a file in the wrong format or with malformed rows.
**Expected Result:** Upload is rejected with a clear error message; no partial/corrupt data is committed.

---

### CFG-20 — Manual add/edit of a single mnemonic
**Steps:** Add or edit a single mnemonic entry via the UI form (if supported alongside bulk upload).
**Expected Result:** Change saves and is reflected in the list.

---

### CFG-21 — Non-admin access restriction (all Asset Configuration screens)
**Preconditions:** Login as non-admin role.
**Steps:** Attempt to navigate directly to each Asset Configuration URL.
**Expected Result:** Access denied/redirected, or read-only depending on business rules.
