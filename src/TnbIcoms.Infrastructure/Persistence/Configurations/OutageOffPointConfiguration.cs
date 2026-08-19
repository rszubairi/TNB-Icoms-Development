using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OutageOffPointConfiguration : IEntityTypeConfiguration<OutageOffPoint>
{
    public void Configure(EntityTypeBuilder<OutageOffPoint> builder)
    {
        builder.ToTable("OutageOffPoints", schema: "dbo");
        builder.HasKey(x => x.OutageOffPointId);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.OffPoints)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Equipment)
            .WithMany()
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
