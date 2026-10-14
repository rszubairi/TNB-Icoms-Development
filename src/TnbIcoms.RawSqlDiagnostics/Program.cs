using System.Diagnostics;
using Microsoft.Data.SqlClient;

if (args.Contains("--help"))
{
    Console.WriteLine("Double-click this executable. It prompts for connection details and runs raw SQL without EF or TNB DLLs.");
    return;
}
string Ask(string prompt, string fallback = "")
{
    Console.Write($"{prompt}{(fallback.Length > 0 ? $" [{fallback}]" : "")}: ");
    var value = Console.ReadLine()?.Trim();
    return string.IsNullOrEmpty(value) ? fallback : value;
}
string Password()
{
    Console.Write("SQL password (hidden): ");
    var value = new System.Text.StringBuilder();
    while (true)
    {
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Enter) break;
        if (key.Key == ConsoleKey.Backspace) { if (value.Length > 0) value.Length--; }
        else if (!char.IsControl(key.KeyChar)) value.Append(key.KeyChar);
    }
    Console.WriteLine();
    return value.ToString();
}
try
{
    Console.WriteLine("Raw SQL diagnostics — SqlClient only; no EF or TNB assemblies.");
    var settings = new SqlConnectionStringBuilder();
    settings.DataSource = Ask("SQL Server (hostname or IP, optionally ,port)");
    settings.InitialCatalog = Ask("Database");
    settings.IntegratedSecurity = Ask("Use Windows authentication? y/n", "n").Equals("y", StringComparison.OrdinalIgnoreCase);
    if (!settings.IntegratedSecurity)
    {
        settings.UserID = Ask("SQL username");
        settings.Password = Password();
    }
    settings.Encrypt = Ask("Encrypt connection? y/n", "y").Equals("y", StringComparison.OrdinalIgnoreCase);
    settings.TrustServerCertificate = Ask("Trust server certificate? y/n", "y").Equals("y", StringComparison.OrdinalIgnoreCase);
    settings.Pooling = Ask("Enable pooling? y/n", "y").Equals("y", StringComparison.OrdinalIgnoreCase);
    settings.ConnectTimeout = 30;
    var id = Ask("AspNetUserId", "f6c9faab-f249-4dd2-ae37-b32a904b7a3b");
    if (!Guid.TryParse(id, out var parsedId)) throw new ArgumentException("AspNetUserId must be a GUID.");
    var repeatsText = Ask("Query repetitions", "10");
    if (!int.TryParse(repeatsText, out var repeats) || repeats < 1 || repeats > 1000)
        throw new ArgumentException("Repetitions must be between 1 and 1000.");
    var heldOpen = Ask("Keep one connection open for all queries? y/n", "n").Equals("y", StringComparison.OrdinalIgnoreCase);
    var sql = """
SELECT TOP(1) [u].[UserId], [u].[AspNetUserId], [u].[AuthType], [u].[CreatedAt], [u].[CreatedBy], [u].[Email], [u].[FullName], [u].[GcuTypeId], [u].[IsActive], [u].[IsDeleted], [u].[OrganisationId], [u].[PhoneNumber], [u].[RoleId], [u].[TnbId], [u].[UpdatedAt], [u].[UpdatedBy], [u].[ZoneId], [r].[RoleId], [r].[CreatedAt], [r].[IsActive], [r].[IsExternal], [r].[RoleCode], [r].[RoleName], [z].[ZoneId], [z].[CreatedAt], [z].[IsActive], [z].[ZoneAbbr], [z].[ZoneName]
FROM [auth].[Users] AS [u]
INNER JOIN [auth].[Roles] AS [r] ON [u].[RoleId] = [r].[RoleId]
INNER JOIN [config].[Zones] AS [z] ON [u].[ZoneId] = [z].[ZoneId]
WHERE [u].[AspNetUserId] = '__ID__' AND [u].[IsActive] = CAST(1 AS bit) AND [u].[IsDeleted] = CAST(0 AS bit)
""".Replace("__ID__", parsedId.ToString());
    var path = Path.Combine(AppContext.BaseDirectory, $"raw-sql-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.log");
    using var file = new StreamWriter(path) { AutoFlush = true };
    void Log(string text) { var line = $"{DateTime.UtcNow:O} {text}"; Console.WriteLine(line); file.WriteLine(line); }
    Log($"Report={path}; Machine={Environment.MachineName}; User={Environment.UserName}; 64bit={Environment.Is64BitProcess}");
    Log($"Server={settings.DataSource}; Database={settings.InitialCatalog}; Encrypt={settings.Encrypt}; TrustCertificate={settings.TrustServerCertificate}; Pooling={settings.Pooling}; HeldOpen={heldOpen}; SqlClient={typeof(SqlConnection).Assembly.GetName().Version}");
    Log($"CommandTimeout=60; No application retries. SQL:\n{sql}");
    var failures = 0;
    SqlConnection CreateConnection()
    {
        var connection = new SqlConnection(settings.ConnectionString);
        connection.FireInfoMessageEventOnUserErrors = false;
        connection.InfoMessage += (_, e) => Log($"SQL INFO: {e.Message}");
        return connection;
    }
    using var shared = heldOpen ? CreateConnection() : null;
    for (var i = 1; i <= repeats; i++)
    {
        using var individual = heldOpen ? null : CreateConnection();
        var connection = shared ?? individual!;
        var watch = Stopwatch.StartNew();
        var stage = "opening connection";
        try
        {
            if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();
            Log($"Iteration={i}; connection opened after {watch.ElapsedMilliseconds}ms; ClientConnectionId={connection.ClientConnectionId}; ServerVersion={connection.ServerVersion}");
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 60;
            stage = "executing query / receiving first result";
            using var reader = await command.ExecuteReaderAsync();
            Log($"Iteration={i}; reader ready after {watch.ElapsedMilliseconds}ms; Columns={reader.FieldCount}");
            stage = "reading result rows";
            var rows = 0;
            while (await reader.ReadAsync())
            {
                // Read every field without exposing returned personal data in the log.
                for (var column = 0; column < reader.FieldCount; column++) _ = reader.GetValue(column);
                rows++;
            }
            Log($"PASS iteration={i}; rows={rows}; total={watch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            failures++;
            Log($"FAIL iteration={i}; stage={stage}; elapsed={watch.ElapsedMilliseconds}ms; {ex}");
            if (ex is SqlException error)
            {
                Log($"ClientConnectionId={error.ClientConnectionId}");
                foreach (SqlError item in error.Errors) Log($"SQL Number={item.Number}; State={item.State}; Severity={item.Class}");
            }
        }
    }
    Log($"Finished: {repeats - failures} passed, {failures} failed. Report: {path}");
    Environment.ExitCode = failures == 0 ? 0 : 1;
}
catch (Exception ex)
{
    Console.WriteLine($"Setup failed: {ex}");
    Environment.ExitCode = 1;
}
finally
{
    Console.WriteLine("Press Enter to close.");
    Console.ReadLine();
}
