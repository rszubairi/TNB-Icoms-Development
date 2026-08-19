using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Auth;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", schema: "auth");
        builder.HasKey(x => x.RoleId);
        builder.Property(x => x.RoleName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.RoleCode).IsRequired().HasMaxLength(30);
    }
}
