using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Auth;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions", schema: "auth");
        builder.HasKey(x => x.RolePermissionId);
        builder.Property(x => x.ModuleCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.PermissionCode).IsRequired().HasMaxLength(30);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
