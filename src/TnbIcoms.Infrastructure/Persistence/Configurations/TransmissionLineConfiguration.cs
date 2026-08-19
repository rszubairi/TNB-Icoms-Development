using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class TransmissionLineConfiguration : IEntityTypeConfiguration<TransmissionLine>
{
    public void Configure(EntityTypeBuilder<TransmissionLine> builder)
    {
        builder.ToTable("TransmissionLines", schema: "config");
        builder.HasKey(x => x.TransmissionLineId);

        builder.HasOne(x => x.VoltageLevel)
            .WithMany()
            .HasForeignKey(x => x.VoltageLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EquipmentType)
            .WithMany()
            .HasForeignKey(x => x.EquipmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
