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
        await SeedDropdownValuesAsync(dbContext);
    }

    /// <summary>
    /// Default dropdown option lists per URS Module 1 §5.2.6 (Table: Dropdown Options).
    /// Existing rows are never overwritten — only missing (Category, ValueCode) pairs are inserted —
    /// so an admin's edits and re-orderings always survive a redeploy/reseed.
    /// </summary>
    private static async Task SeedDropdownValuesAsync(AppDbContext dbContext)
    {
        var jobTypeLabels = new[]
        {
            "Conditional Monitoring", "Defect Correction", "Fault Investigation", "Inspection",
            "Routine Maintenance", "SCADA & SAS", "Testing", "Projects", "Others",
            "Distribution Work", "Transfer of Asset", "Repair and Rehabilitation"
        };

        // Kept in sync with TnbIcoms.Application.DropdownValues.DropdownCategories.OutageTypeParents.
        var outageTypeParents = new[] { "Planned", "Unplanned", "Emergency", "Forced" };

        var seeds = new List<(string Category, string Label, string? ParentCode, string? CodeOverride)>();
        foreach (var outageType in outageTypeParents)
        {
            seeds.AddRange(jobTypeLabels.Select(label =>
                ("JobType", label, (string?)outageType, (string?)$"{outageType}_{ToValueCode(label)}")));
        }

        seeds.AddRange(new (string, string, string?, string?)[]
        {
            ("WorkType", "Live", null, null),
            ("WorkType", "Dead", null, null),

            ("Sequence", "One at a time", null, null),
            ("Sequence", "All at the same time", null, null),
            ("Sequence", "Not applicable", null, null),

            ("Restoration", "Immediately", null, null),
            ("Restoration", "15 Minutes", null, null),
            ("Restoration", "30 Minutes", null, null),
            ("Restoration", "45 Minutes", null, null),
            ("Restoration", "1 Hour", null, null),
            ("Restoration", "1.5 Hours", null, null),
            ("Restoration", "2 Hours", null, null),
            ("Restoration", "More than 2 Hours", null, null),
            ("Restoration", "Not Applicable", null, null),

            ("GcuType", "Data Centre", null, null),
            ("GcuType", "Large Scale Solar", null, null),
            ("GcuType", "Large Power Consumer", null, null),

            ("MvaRating", "240 MVA", null, null),
            ("MvaRating", "180 MVA", null, null),
            ("MvaRating", "90 MVA", null, null),
            ("MvaRating", "245 MVA", null, null),
            ("MvaRating", "30 MVA", null, null)
        });

        var existingKeys = (await dbContext.DropdownValues
                .Select(d => new { d.CategoryCode, d.ValueCode })
                .ToListAsync())
            .Select(d => (d.CategoryCode, d.ValueCode))
            .ToHashSet();

        var sortOrders = await dbContext.DropdownValues
            .GroupBy(d => d.CategoryCode)
            .Select(g => new { Category = g.Key, MaxSortOrder = g.Max(d => d.SortOrder) })
            .ToDictionaryAsync(g => g.Category, g => g.MaxSortOrder);

        foreach (var (category, label, parentCode, codeOverride) in seeds)
        {
            var code = codeOverride ?? ToValueCode(label);
            if (!existingKeys.Add((category, code)))
            {
                continue;
            }

            var nextSortOrder = sortOrders.TryGetValue(category, out var max) ? max + 1 : 1;
            sortOrders[category] = nextSortOrder;

            dbContext.DropdownValues.Add(new DropdownValue
            {
                CategoryCode = category,
                ValueCode = code,
                ValueLabel = label,
                ParentCode = parentCode,
                SortOrder = nextSortOrder,
                IsActive = true
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static string ToValueCode(string label)
    {
        var cleaned = new string(label.Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray());
        return cleaned.Replace(" ", "_").ToUpperInvariant();
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
