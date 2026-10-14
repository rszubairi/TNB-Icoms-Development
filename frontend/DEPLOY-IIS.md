# IIS deployment

Frontend URL: `http://10.52.9.20/` (or `http://localhost/` on the server).
Backend API URL compiled into this build: `http://10.52.9.20:8080/api`.

## Frontend

1. Install the IIS Static Content feature and Microsoft IIS URL Rewrite module.
2. Back up the existing frontend site files.
3. Copy all contents of `dist/frontend/browser/` into the frontend IIS site's physical root, including `web.config`. Put `index.html` directly in that root, without an extra `browser` folder.
4. Use an HTTP site binding on port 80 that accepts the server IP.
5. Open the frontend, sign in, and refresh a nested application route to verify routing.

The supplied `web.config` is for the separate frontend IIS site. Do not overwrite the backend ASP.NET Core site's `web.config` with this file.

## Backend prerequisites

The backend is deployed separately. Configure its IIS site to serve HTTP on port 8080 and ensure client machines can reach that port.

The existing CORS policy in `src/TnbIcoms.Api/Program.cs` currently allows only development frontend origins. Before publishing the backend, add these origins to its existing `WithOrigins(...)` list:

```csharp
"http://10.52.9.20", "http://localhost"
```

Origins have no trailing slash. Different ports are different origins even on the same server.

The backend currently calls `UseHttpsRedirection()`. Ensure the deployed HTTP endpoint does not redirect API requests or CORS preflight requests to an unconfigured HTTPS endpoint. If HTTPS is required by the server, configure the proper HTTPS endpoint and rebuild the frontend with that API URL.

## Rebuild

From the `frontend` folder:

```powershell
npm run build -- --configuration production
```

The API URL is set in `src/environments/environment.ts`. Running `set-env.js` separately overwrites that file; if using that script, set `NG_APP_API_URL` to `http://10.52.9.20:8080/api` first.
