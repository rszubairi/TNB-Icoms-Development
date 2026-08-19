using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Identity;

namespace TnbIcoms.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public const string DefaultAdminEmail = "admin@tnb.local";
    public const string DefaultAdminPassword = "Admin@12345";
    public const string DefaultAdminTnbId = "10000001";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await dbContext.Database.MigrateAsync();

        var roleSeeds = new (string Name, string Code, bool IsExternal)[]
        {
            ("SysAdmin", "SYSADMIN", false),
            ("GNM Admin", "GNM_ADMIN", false),
            ("Requestor / Planner", "PLANNER", false),
            ("TOMS / GNM", "TOMS", false),
            ("GNC", "GNC", false),
            ("GCU", "GCU", true)
        };

        foreach (var seed in roleSeeds)
        {
            if (!await dbContext.AppRoles.AnyAsync(r => r.RoleCode == seed.Code))
            {
                dbContext.AppRoles.Add(new Role
                {
                    RoleName = seed.Name,
                    RoleCode = seed.Code,
                    IsExternal = seed.IsExternal,
                    IsActive = true
                });
            }
        }
        await dbContext.SaveChangesAsync();

        var sysAdminRole = await dbContext.AppRoles.FirstAsync(r => r.RoleCode == "SYSADMIN");

        var centralZone = await dbContext.Zones.FirstOrDefaultAsync(z => z.ZoneAbbr == "CTL");
        if (centralZone is null)
        {
            centralZone = new Zone { ZoneName = "Central", ZoneAbbr = "CTL", IsActive = true };
            dbContext.Zones.Add(centralZone);
            await dbContext.SaveChangesAsync();
        }

        if (!await roleManager.RoleExistsAsync(sysAdminRole.RoleCode))
        {
            await roleManager.CreateAsync(new IdentityRole(sysAdminRole.RoleCode));
        }

        var identityAdmin = await userManager.FindByEmailAsync(DefaultAdminEmail);
        if (identityAdmin is null)
        {
            identityAdmin = new ApplicationUser
            {
                UserName = DefaultAdminEmail,
                Email = DefaultAdminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                TnbId = DefaultAdminTnbId,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(identityAdmin, DefaultAdminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed admin identity user: {errors}");
            }

            await userManager.AddToRoleAsync(identityAdmin, sysAdminRole.RoleCode);
        }

        var domainAdmin = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.AspNetUserId == identityAdmin.Id);
        if (domainAdmin is null)
        {
            dbContext.AppUsers.Add(new User
            {
                TnbId = DefaultAdminTnbId,
                FullName = "System Administrator",
                Email = DefaultAdminEmail,
                AuthType = 1,
                AspNetUserId = identityAdmin.Id,
                RoleId = sysAdminRole.RoleId,
                ZoneId = centralZone.ZoneId,
                IsActive = true,
                CreatedBy = 0
            });
            await dbContext.SaveChangesAsync();
        }

        await SeedEmailTemplatesAsync(dbContext);
    }

    /// <summary>
    /// Default content for the templates IT Admin can self-manage under Email Templates.
    /// Existing rows are never overwritten here — only missing TemplateCodes are inserted —
    /// so an admin's edits always survive a redeploy/reseed.
    /// </summary>
    private static async Task SeedEmailTemplatesAsync(AppDbContext dbContext)
    {
        var templateSeeds = new (string Code, string Name, string Subject, string Body, string Tags)[]
        {
            (
                "OutageStatusNotification",
                "Outage Status Notification (to PIC)",
                "Outage {{OutageNumber}} {{Action}}",
                "<p>Outage {{OutageNumber}} has been {{Action}}.</p><p>Log in to ICOMS 2.0 for details.</p>",
                "OutageNumber,Action"
            ),
            (
                "GncAuthorisationStatus",
                "GNC Authorisation Status (to Authoriser)",
                "Outage {{OutageNumber}} — {{Status}}",
                "<p>Outage {{OutageNumber}} has been marked {{Status}}.</p><p>Log in to ICOMS 2.0 for details.</p>",
                "OutageNumber,Status"
            ),
            (
                "RoleTransferRequestSubmitted",
                "Role Transfer Request Submitted (to SysAdmin)",
                "{{RequestingUserFullName}} requests change in User Role/Zone",
                "<p>{{RequestingUserFullName}} has requested a change in their User Role/Zone to {{NewRole}} + {{NewZone}} with the following justification.</p><p>{{Reason}}</p><p>This is an automated email by ICOMS 2.0.</p>",
                "RequestingUserFullName,NewRole,NewZone,Reason"
            ),
            (
                "UserWelcome",
                "New User Welcome",
                "Welcome to TNB ICOMS 2.0",
                "<p>Hi {{FullName}},</p><p>Your account has been created. Temporary password: <b>{{TemporaryPassword}}</b></p>",
                "FullName,TemporaryPassword"
            )
        };

        foreach (var seed in templateSeeds)
        {
            if (!await dbContext.EmailTemplates.AnyAsync(t => t.TemplateCode == seed.Code))
            {
                dbContext.EmailTemplates.Add(new EmailTemplate
                {
                    TemplateCode = seed.Code,
                    Name = seed.Name,
                    Subject = seed.Subject,
                    BodyHtml = seed.Body,
                    AvailableTags = seed.Tags,
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        await dbContext.SaveChangesAsync();
    }
}
