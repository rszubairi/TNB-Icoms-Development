using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnbIcoms.Infrastructure.Identity;
using TnbIcoms.Infrastructure.Persistence;

var interactive = args.Length == 0;
if (interactive)
{
    Console.WriteLine("ICOMs database diagnostics (read-only)");
    string Ask(string prompt, string defaultValue = "")
    {
        Console.Write($"{prompt}{(defaultValue.Length > 0 ? $" [{defaultValue}]" : "")}: ");
        var answer = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(answer) ? defaultValue : answer.Trim('"');
    }
    var folder = Ask("Deployed API folder containing appsettings.json", AppContext.BaseDirectory);
    while (!File.Exists(Path.Combine(folder, "appsettings.json")))
    {
        Console.WriteLine("appsettings.json was not found in that folder.");
        folder = Ask("Deployed API folder");
    }
    var staff = Ask("Staff ID or email");
    while (string.IsNullOrWhiteSpace(staff)) staff = Ask("Staff ID or email (required)");
    var env = Ask("API environment", "Production");
    var repeats = Ask("Number of queries (1-1000)", "10");
    while (!int.TryParse(repeats, out var n) || n < 1 || n > 1000)
        repeats = Ask("Enter a number between 1 and 1000", "10");
    var retry = Ask("Enable EF retries? y/n", "n");
    var pooling = Ask("Enable connection pooling? y/n", "y");
    var inputs = new List<string> { folder, staff, "--environment", env, "--count", repeats };
    if (retry.Equals("y", StringComparison.OrdinalIgnoreCase)) inputs.Add("--retry");
    if (pooling.Equals("n", StringComparison.OrdinalIgnoreCase)) inputs.Add("--no-pooling");
    args = inputs.ToArray();
}
if (args.Length < 2 || args.Contains("--help"))
{
    Console.WriteLine("Usage: TnbIcoms.DbDiagnostics <API-folder> <staff-id-or-email> [--environment Production] [--count 10] [--retry] [--no-pooling]");
    return;
}
try
{
string Option(string name, string fallback) => Array.IndexOf(args, name) is var i && i >= 0 && i + 1 < args.Length ? args[i + 1] : fallback;
var apiFolder = Path.GetFullPath(args[0]);
var login = args[1];
var environment = Option("--environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");
var count = int.Parse(Option("--count", "10"));
if (count < 1 || count > 1000) throw new ArgumentException("Count must be between 1 and 1000.");
var config = new ConfigurationBuilder().SetBasePath(apiFolder)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables().Build();
var connectionString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");
var settings = new SqlConnectionStringBuilder(connectionString);
if (args.Contains("--no-pooling")) settings.Pooling = false;
connectionString = settings.ConnectionString;
var logPath = Path.Combine(AppContext.BaseDirectory, $"diagnostics-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.log");
using var logFile = new StreamWriter(logPath) { AutoFlush = true };
var logLock = new object();
void Log(string message)
{
    lock (logLock)
    {
        var line = $"{DateTime.UtcNow:O} {message}";
        Console.WriteLine(line);
        logFile.WriteLine(line);
    }
}
// Avoid printing credentials, parameter values, user records, or access tokens.
Log($"Machine={Environment.MachineName}; OS-user={Environment.UserName}; process64bit={Environment.Is64BitProcess}; environment={environment}");
Log($"Server={settings.DataSource}; Database={settings.InitialCatalog}; Pooling={settings.Pooling}; Encrypt={settings.Encrypt}; TrustServerCertificate={settings.TrustServerCertificate}");
Log($"Infrastructure={typeof(AppDbContext).Assembly.Location}; Version={typeof(AppDbContext).Assembly.GetName().Version}; EF={typeof(DbContext).Assembly.GetName().Version}; SqlClient={typeof(SqlConnection).Assembly.GetName().Version}");
Log($"Retries={args.Contains("--retry")}; CommandTimeout=60; Report={logPath}");
var failures = 0;
async Task Test(string name, Func<Task> action)
{
    var timer = Stopwatch.StartNew();
    Log($"START {name}");
    try { await action(); Log($"PASS {name} ({timer.ElapsedMilliseconds}ms)"); }
    catch (Exception ex)
    {
        failures++;
        Log($"FAIL {name} ({timer.ElapsedMilliseconds}ms): {ex}");
        for (Exception? current = ex; current != null; current = current.InnerException)
            if (current is SqlException sql)
            {
                Log($"ClientConnectionId={sql.ClientConnectionId}");
                foreach (SqlError error in sql.Errors)
                    Log($"SQL Number={error.Number}; State={error.State}; Class={error.Class}; Procedure={error.Procedure}; Line={error.LineNumber}");
            }
    }
}
async Task Raw(SqlConnection connection, string sql, string? userId = null)
{
    using var command = connection.CreateCommand();
    command.CommandText = sql;
    command.CommandTimeout = 60;
    if (userId != null) command.Parameters.Add(new SqlParameter("@id", System.Data.SqlDbType.NVarChar, 450) { Value = userId });
    var timer = Stopwatch.StartNew();
    using var reader = await command.ExecuteReaderAsync();
    var rows = 0;
    while (await reader.ReadAsync()) rows++;
    Log($"Raw rows={rows}; elapsed={timer.ElapsedMilliseconds}ms; ClientConnectionId={connection.ClientConnectionId}");
}
await Test("Raw: open + SELECT 1", async () =>
{
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    await Raw(connection, "SELECT 1;");
});
await Test("Raw: repeated queries on one held-open connection", async () =>
{
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    for (var i = 1; i <= count; i++) { Log($"Iteration {i}/{count}"); await Raw(connection, "SELECT 1;"); }
});
for (var i = 1; i <= count; i++)
    await Test($"Raw: open/query/close {i}/{count}", async () =>
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await Raw(connection, "SELECT 1;");
    });

var services = new ServiceCollection();
var capture = new CommandCapture();
services.AddLogging();
services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString, sql =>
{
    sql.CommandTimeout(60);
    if (args.Contains("--retry")) sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
}).AddInterceptors(capture).EnableDetailedErrors().LogTo(Log, LogLevel.Debug));
services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<AppDbContext>();
using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
string? identityId = null;
await Test("EF/Identity: same LoginAd identity lookup", async () =>
{
    var identity = await manager.FindByEmailAsync(login)
        ?? await manager.Users.FirstOrDefaultAsync(u => u.TnbId == login);
    identityId = identity?.Id;
    Log($"Identity found={identity != null}; active={identity?.IsActive}; ContextId={db.ContextId}");
});
if (identityId != null)
{
    for (var i = 1; i <= count; i++)
        await Test($"EF: same LoginAd domain lookup {i}/{count}", async () =>
        {
            var query = db.AppUsers.Include(u => u.Role).Include(u => u.Zone)
                .Where(u => u.AspNetUserId == identityId && u.IsActive && !u.IsDeleted);
            var user = await query.FirstOrDefaultAsync();
            Log($"Domain user found={user != null}; ContextId={db.ContextId}");
        });
    var capturedSql = capture.Sql;
    var capturedParameters = capture.Parameters;
    if (capturedSql != null)
        await Test("Raw: replay exact captured EF command and parameters", async () =>
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = capturedSql;
            command.CommandTimeout = 60;
            foreach (var parameter in capturedParameters)
                command.Parameters.Add((SqlParameter)((ICloneable)parameter).Clone());
            Log($"Captured EF SQL: {capturedSql}");
            using var reader = await command.ExecuteReaderAsync();
            var rows = 0;
            while (await reader.ReadAsync()) rows++;
            Log($"Exact replay rows={rows}; ClientConnectionId={connection.ClientConnectionId}");
        });
    await Test("Raw: equivalent LoginAd join", async () =>
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await Raw(connection, "SELECT TOP(1) u.*, r.*, z.* FROM auth.Users u INNER JOIN auth.Roles r ON u.RoleId=r.RoleId INNER JOIN config.Zones z ON u.ZoneId=z.ZoneId WHERE u.AspNetUserId=@id AND u.IsActive=1 AND u.IsDeleted=0;", identityId);
    });
}
else Log("SKIP domain lookups: identity lookup failed or no matching user was found.");
Log($"COMPLETE Failures={failures}");
Environment.ExitCode = failures == 0 ? 0 : 1;
}
catch (Exception ex)
{
    Console.WriteLine($"Diagnostic setup failed: {ex}");
    Environment.ExitCode = 1;
}
finally
{
    if (interactive)
    {
        Console.WriteLine("Press Enter to close.");
        Console.ReadLine();
    }
}

sealed class CommandCapture : DbCommandInterceptor
{
    public string? Sql { get; private set; }
    public SqlParameter[] Parameters { get; private set; } = [];
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        Sql = command.CommandText;
        Parameters = command.Parameters.Cast<SqlParameter>()
            .Select(p => (SqlParameter)((ICloneable)p).Clone()).ToArray();
        return ValueTask.FromResult(result);
    }
}
