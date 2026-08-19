using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class TransmissionLineOwnerZoneConfiguration : IEntityTypeConfiguration<TransmissionLineOwnerZone>
{
    public void Configure(EntityTypeBuilder<TransmissionLineOwnerZone> builder)
    {
        builder.ToTable("TransmissionLineOwnerZones", schema: "config");
        builder.HasKey(x => x.TransmissionLineOwnerZoneId);
        builder.HasIndex(x => new { x.TransmissionLineId, x.ZoneId }).IsUnique();

        builder.HasOne(x => x.TransmissionLine)
            .WithMany(x => x.OwnerZones)
            .HasForeignKey(x => x.TransmissionLineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Zone)
            .WithMany()
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
