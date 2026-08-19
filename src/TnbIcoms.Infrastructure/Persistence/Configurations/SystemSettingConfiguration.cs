using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings", schema: "config");
        builder.HasKey(x => x.SystemSettingId);
        builder.Property(x => x.SettingKey).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SettingValue).IsRequired().HasMaxLength(500);
        builder.HasIndex(x => x.SettingKey).IsUnique();
    }
}
