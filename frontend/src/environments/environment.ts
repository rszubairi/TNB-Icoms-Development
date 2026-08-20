export const environment = {
  production: true,
  // Points at the .NET Web API (src/TnbIcoms.Api). Port is configurable — 5000 is the local dev default.
  apiUrl: 'http://localhost:5000/api',
  // Hides/shows the "Corporate AD / SSO" login button. Must match the backend's Ad:Enabled setting.
  adLoginEnabled: true
};
