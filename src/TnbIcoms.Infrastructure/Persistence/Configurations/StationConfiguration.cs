using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Stations", schema: "config");
        builder.HasKey(x => x.StationId);
        builder.Property(x => x.StationName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.StationAbbr).IsRequired().HasMaxLength(20);
        builder.HasIndex(x => x.StationName).IsUnique();
        builder.HasIndex(x => x.StationAbbr).IsUnique();

        builder.HasOne(x => x.Zone)
            .WithMany(x => x.Stations)
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Organisation)
            .WithMany()
            .HasForeignKey(x => x.OrgId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
