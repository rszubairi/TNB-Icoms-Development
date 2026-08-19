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
            if (!await dbContext.Roles.AnyAsync(r => r.RoleCode == seed.Code))
            {
                dbContext.Roles.Add(new Role
                {
                    RoleName = seed.Name,
                    RoleCode = seed.Code,
                    IsExternal = seed.IsExternal,
                    IsActive = true
                });
            }
        }
        await dbContext.SaveChangesAsync();

        var sysAdminRole = await dbContext.Roles.FirstAsync(r => r.RoleCode == "SYSADMIN");

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

        var domainAdmin = await dbContext.Users.FirstOrDefaultAsync(u => u.AspNetUserId == identityAdmin.Id);
        if (domainAdmin is null)
        {
            dbContext.Users.Add(new User
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
    }
}
