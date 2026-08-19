using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OutageAdditionalEquipmentConfiguration : IEntityTypeConfiguration<OutageAdditionalEquipment>
{
    public void Configure(EntityTypeBuilder<OutageAdditionalEquipment> builder)
    {
        builder.ToTable("OutageAdditionalEquipment", schema: "dbo");
        builder.HasKey(x => x.OutageAdditionalEquipmentId);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.AdditionalEquipment)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Equipment)
            .WithMany()
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
