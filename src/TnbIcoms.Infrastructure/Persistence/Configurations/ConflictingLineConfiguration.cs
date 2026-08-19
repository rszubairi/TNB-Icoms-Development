using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class ConflictingLineConfiguration : IEntityTypeConfiguration<ConflictingLine>
{
    public void Configure(EntityTypeBuilder<ConflictingLine> builder)
    {
        builder.ToTable("ConflictingLines", schema: "config");
        builder.HasKey(x => x.ConflictingLineId);

        builder.HasOne(x => x.Equipment)
            .WithMany()
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ConflictingEquipment)
            .WithMany()
            .HasForeignKey(x => x.ConflictingEquipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
