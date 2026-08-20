# UAT Script 01 — Authentication & Login

**View(s):** Login (`/login`) — `features/auth/login`
**API:** `POST /api/auth/login`, `POST /api/auth/login-ad`, `POST /api/auth/refresh`, `POST /api/auth/logout`

## Preconditions
- Application URL accessible, not authenticated (clear cookies/local storage first).
- Valid test credentials available for at least one Local user and, if `adLoginEnabled` is true in the target environment, one AD/SSO user.

---

### LOGIN-01 — Successful login with valid local credentials
**Priority:** High
**Steps:**
1. Navigate to `/login`.
2. Enter a valid Staff ID/Email in "Staff ID / Email".
3. Enter the correct password.
4. Click **Sign In**.

**Expected Result:** Button shows "Signing in…" while submitting, then user is redirected to `/admin/users` (or role-appropriate landing page). Sidebar and header render with the logged-in user's name.

---

### LOGIN-02 — Login fails with incorrect password
**Steps:**
1. Navigate to `/login`.
2. Enter a valid Staff ID/Email.
3. Enter an incorrect password.
4. Click **Sign In**.

**Expected Result:** Login is rejected; an error message is displayed in the form alert area (from `err.error.error` or fallback message). User remains on `/login`. No token is stored.

---

### LOGIN-03 — Login fails with unknown identifier
**Steps:** Enter a Staff ID/Email that does not exist, any password, click **Sign In**.
**Expected Result:** Generic error shown (should not reveal whether the account exists, for security). User remains on `/login`.

---

### LOGIN-04 — Required field validation
**Steps:**
1. Leave "Staff ID / Email" blank, click into and out of the field (or attempt submit).
2. Leave "Password" blank, click into and out of the field.

**Expected Result:** Inline validation errors appear under each empty required field once touched. **Sign In** does not submit while required fields are empty/invalid.

---

### LOGIN-05 — "Remember me" checkbox
**Steps:** Log in successfully with "Remember me" checked.
**Expected Result:** Confirm with dev/backend team the actual persistence behaviour (e.g. longer-lived refresh token). Document actual behaviour observed; flag as a defect if no observable difference exists.

---

### LOGIN-06 — Corporate AD / SSO login (only if enabled in environment)
**Preconditions:** `environment.adLoginEnabled = true` for the target environment; AD test account available.
**Steps:**
1. Navigate to `/login`. Confirm the **Corporate AD / SSO** button is visible.
2. Click **Corporate AD / SSO**.
3. Complete the AD/SSO authentication flow.

**Expected Result:** User is authenticated via `POST /api/auth/login-ad`, redirected to the application landing page with an AD-linked session.

---

### LOGIN-07 — AD/SSO button hidden when disabled
**Preconditions:** `environment.adLoginEnabled = false`.
**Steps:** Navigate to `/login`.
**Expected Result:** **Corporate AD / SSO** button is not rendered on the page.

---

### LOGIN-08 — "Forgot password?" link (known gap)
**Steps:** Click **Forgot password?** on the login page.
**Expected Result (current behaviour):** Link performs no action (`javascript:void(0)`). **Action for UAT:** confirm with business whether password reset should be implemented in this cycle; log as defect/enhancement if expected.

---

### LOGIN-09 — Session persistence after page refresh
**Steps:** Log in successfully, then refresh the browser (F5) on any authenticated page.
**Expected Result:** User remains logged in (session/token restored via `POST /api/auth/refresh` or stored token), not redirected back to `/login`.

---

### LOGIN-10 — Route guard blocks unauthenticated access
**Steps:** Without logging in, directly navigate to an internal route, e.g. `/admin/users`.
**Expected Result:** `authGuard` redirects the browser to `/login`. The originally requested page is not rendered.

---

### LOGIN-11 — Logout
**Preconditions:** Logged in.
**Steps:** Trigger logout from the header/profile menu.
**Expected Result:** `POST /api/auth/logout` is called, session/token is cleared, and the user is redirected to `/login`. Attempting to navigate back (browser Back button) to an authenticated page redirects to `/login` again.

---

### LOGIN-12 — Expired/invalid token handling
**Preconditions:** A stored token that is expired or has been invalidated server-side.
**Steps:** Load the application or perform an action requiring the API.
**Expected Result:** User is gracefully redirected to `/login` rather than seeing a raw error or blank screen; the `auth.interceptor.ts` handles the 401 response.
