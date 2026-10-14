using Microsoft.Data.SqlClient;
using System.Text.Json;
using TnbIcoms.Application.Auth.Dtos;
using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Domain.Entities.Config;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, ContentRootPath = AppContext.BaseDirectory });
builder.WebHost.UseUrls("http://127.0.0.1:5099");
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Missing ConnectionStrings:Default.");
var app = builder.Build();
app.MapPost("/login-ad-query", async (Lookup request, ILogger<Lookup> logger) =>
{
    if (string.IsNullOrWhiteSpace(request.StaffIdOrEmail)) return Results.BadRequest("Staff ID or email required.");
    try
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var result = new GatewayLookupResponse();
        using (var command = connection.CreateCommand())
        {
            command.CommandTimeout = 60;
            command.CommandText = "SELECT TOP(1) Id, Email, TnbId, IsActive FROM dbo.AspNetUsers WHERE NormalizedEmail=@value";
            command.Parameters.AddWithValue("@value", request.StaffIdOrEmail.ToUpperInvariant());
            async Task<GatewayIdentity?> Read()
            {
                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;
                return new GatewayIdentity { Id = reader.GetString(0), Email = reader.IsDBNull(1) ? null : reader.GetString(1), TnbId = reader.IsDBNull(2) ? null : reader.GetString(2), IsActive = reader.GetBoolean(3) };
            }
            result.Identity = await Read();
            if (result.Identity == null)
            {
                command.CommandText = "SELECT TOP(1) Id, Email, TnbId, IsActive FROM dbo.AspNetUsers WHERE TnbId=@value";
                command.Parameters[0].Value = request.StaffIdOrEmail;
                result.Identity = await Read();
            }
        }
        if (result.Identity is { IsActive: true })
        {
            using var command = connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText = "SELECT TOP(1) u.*, r.RoleName, r.RoleCode, r.IsExternal, r.IsActive AS RoleIsActive, r.CreatedAt AS RoleCreatedAt, z.ZoneName, z.ZoneAbbr, z.IsActive AS ZoneIsActive, z.CreatedAt AS ZoneCreatedAt FROM auth.Users u INNER JOIN auth.Roles r ON u.RoleId=r.RoleId INNER JOIN config.Zones z ON u.ZoneId=z.ZoneId WHERE u.AspNetUserId=@id AND u.IsActive=1 AND u.IsDeleted=0";
            command.Parameters.Add(new SqlParameter("@id", System.Data.SqlDbType.NVarChar, 450) { Value = result.Identity.Id });
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var values = new Dictionary<string, object?>();
                for (var i = 0; i < reader.FieldCount; i++) values[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                var user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(values))!;
                user.Role = new Role { RoleId = user.RoleId, RoleName = (string)values["RoleName"]!, RoleCode = (string)values["RoleCode"]!, IsExternal = (bool)values["IsExternal"]!, IsActive = (bool)values["RoleIsActive"]!, CreatedAt = (DateTime)values["RoleCreatedAt"]! };
                user.Zone = new Zone { ZoneId = user.ZoneId, ZoneName = (string)values["ZoneName"]!, ZoneAbbr = (string)values["ZoneAbbr"]!, IsActive = (bool)values["ZoneIsActive"]!, CreatedAt = (DateTime)values["ZoneCreatedAt"]! };
                result.DomainUser = user;
            }
        }
        return Results.Ok(result);
    }
    catch (Exception ex) { logger.LogError(ex, "Gateway lookup failed"); return Results.Json(new { error = ex.Message }, statusCode: 500); }
});
await app.RunAsync();
record Lookup(string StaffIdOrEmail);
