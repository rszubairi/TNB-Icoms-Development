# Database diagnostics

## Standalone Windows executable (no dotnet installation required)

Copy the entire `artifacts/db-diagnostics-win-x64` folder to a 64-bit Windows server and double-click `TnbIcoms.DbDiagnostics.exe`. It prompts for the deployed API folder, staff ID/email, environment, query count, retries and pooling. Press Enter to accept defaults. The window stays open after completion or a setup error.

Build this distribution with:

```powershell
dotnet publish src/TnbIcoms.DbDiagnostics/TnbIcoms.DbDiagnostics.csproj -c Release -r win-x64 --self-contained true -o artifacts/db-diagnostics-win-x64
```

The executable ships with its runtime and dependencies: transfer the entire folder, not only the EXE. No command-line parameters are needed.

Read-only console diagnostics using the same Infrastructure/AppDbContext DLL, Identity store, EF Core and SqlClient dependencies as the API. It does not authenticate against AD, issue tokens, modify data or run migrations.

## Publish

From the repository root:

```powershell
dotnet publish src/TnbIcoms.DbDiagnostics/TnbIcoms.DbDiagnostics.csproj -c Release -o artifacts/db-diagnostics
```

Copy the **entire** output folder to the IIS server. It requires the .NET 8 and ASP.NET Core 8 runtimes. Build this tool and the API from the same checkout so their shared DLLs match; the tool logs assembly paths and versions. It does not automatically load DLLs from the deployed API folder.

## Run on the IIS server

Run from the diagnostics folder, supplying the deployed API directory and a real staff ID or email:

```powershell
dotnet .\TnbIcoms.DbDiagnostics.dll "C:\inetpub\wwwroot\IcomsApi" "STAFF-ID" --count 10
dotnet .\TnbIcoms.DbDiagnostics.dll "C:\inetpub\wwwroot\IcomsApi" "STAFF-ID" --count 10 --retry
dotnet .\TnbIcoms.DbDiagnostics.dll "C:\inetpub\wwwroot\IcomsApi" "STAFF-ID" --count 10 --no-pooling
```

Configuration loads from API `appsettings.json`, `appsettings.Production.json` (default), then environment variables, including `ConnectionStrings__Default`. Use `--environment Development` or another environment to match IIS. Environment variables configured only in IIS/web.config are not inherited by a separate terminal: set matching overrides in the terminal if applicable. Connection credentials are never printed. Reports may contain SQL, server names and exception details; review before sharing.

Each run writes a timestamped log beside the console DLL and prints its path. Exit code 1 means one or more tests failed. A missing Identity record skips domain tests and is explicitly reported.

Tests: SELECT 1; repeated SELECT 1 on one held-open connection; repeated open/query/close; actual UserManager identity lookup; current LoginAd domain EF query repeatedly; raw replay of the last EF command with its original parameters; equivalent handwritten join. EF debug logs include SQL commands, timings, connections and retries, with sensitive-data logging disabled.

Baseline has no EF retries to expose the original failure. `--retry` matches API retries (3, maximum 10-second delay). All commands use the API's 60-second command timeout. `--no-pooling` changes only connection pooling and helps compare pool behavior. No result records or passwords are printed.

Compare report failure stage, elapsed time, SQL error number and ClientConnectionId. If held-open succeeds but open/close fails, investigate connection establishment/pooling. If EF and exact raw replay both fail, investigate SQL/network behavior rather than EF materialization. If raw replay succeeds and EF fails repeatedly, inspect mapping/materialization details in the EF exception. These comparisons provide evidence, not proof of a particular cause.

The console runs as the terminal user, not automatically as the IIS application-pool identity. Integrated authentication, environment and network permissions can differ. EF logs are client-side logs, not the SQL Server ERRORLOG; correlate UTC timestamps and ClientConnectionId with server diagnostics.
