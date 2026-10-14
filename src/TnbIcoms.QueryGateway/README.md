# LoginAd gateway test

Copy all files from `artifacts/query-gateway-win-x64` together. Edit its `appsettings.json` ConnectionStrings:Default to use the local SQL instance and database. The supplied default uses Windows authentication as the account running the executable. For SQL authentication replace Integrated Security=True with User Id=...;Password=... . Restart the executable after changing settings. No prompts or client executable are needed.

Start TnbIcoms.QueryGateway.exe. It listens on http://127.0.0.1:5099. There is no source-IP restriction or test-key requirement in this loopback-only test build.

POST api/auth/login-ad now calls LoginWithAdGatewayAsync. It obtains the Identity lookup and complete active domain user with nested Role and Zone from the gateway. Passwords are not sent to the gateway. Existing AD credential validation, bypass settings and JWT response generation remain in the API. Gateway errors flow into the API exception middleware.

The API defaults to http://127.0.0.1:5099; an optional Gateway:BaseUrl configuration overrides its client URL. Loopback is local to each machine: this build requires API and gateway on the same machine, or a separately configured tunnel. Setting the API URL to the DB server IP alone will not reach this loopback listener. For separate web/DB server testing, the listener must later be changed to a reachable interface with appropriate access controls.

Republish the API as well as the gateway. The gateway uses raw SqlClient and the shared Domain DTO shapes, without EF database access. The old direct SQL service method is retained for comparison. The old GatewayTest executable is not used by this flow.
