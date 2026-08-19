using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class SingleLineDiagramConfiguration : IEntityTypeConfiguration<SingleLineDiagram>
{
    public void Configure(EntityTypeBuilder<SingleLineDiagram> builder)
    {
        builder.ToTable("SingleLineDiagrams", schema: "config");
        builder.HasKey(x => x.SingleLineDiagramId);
        builder.Property(x => x.FlowType).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30);
        builder.Property(x => x.Mnemonic).HasMaxLength(50);
        builder.Property(x => x.SubstationType).HasMaxLength(20);
        builder.Property(x => x.DiagramNumber).HasMaxLength(100);

        builder.HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.VoltageLevel)
            .WithMany()
            .HasForeignKey(x => x.VoltageLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
