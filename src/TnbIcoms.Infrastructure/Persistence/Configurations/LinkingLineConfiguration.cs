using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class LinkingLineConfiguration : IEntityTypeConfiguration<LinkingLine>
{
    public void Configure(EntityTypeBuilder<LinkingLine> builder)
    {
        builder.ToTable("LinkingLines", schema: "config");
        builder.HasKey(x => x.LinkingLineId);

        builder.HasOne(x => x.Equipment)
            .WithMany()
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LinkedEquipment)
            .WithMany()
            .HasForeignKey(x => x.LinkedEquipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
