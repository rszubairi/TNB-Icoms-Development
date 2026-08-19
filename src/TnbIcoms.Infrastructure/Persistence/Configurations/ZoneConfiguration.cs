using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("Zones", schema: "config");
        builder.HasKey(x => x.ZoneId);
        builder.Property(x => x.ZoneName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.ZoneAbbr).IsRequired().HasMaxLength(20);
    }
}
