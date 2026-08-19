using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class ZoneLocationConfiguration : IEntityTypeConfiguration<ZoneLocation>
{
    public void Configure(EntityTypeBuilder<ZoneLocation> builder)
    {
        builder.ToTable("ZoneLocations", schema: "config");
        builder.HasKey(x => x.ZoneLocationId);
        builder.Property(x => x.LocationName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
        builder.Property(x => x.Longitude).HasColumnType("decimal(9,6)");

        builder.HasOne(x => x.Zone)
            .WithMany(x => x.Locations)
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
