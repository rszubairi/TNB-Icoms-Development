using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class SingleLineDiagramConfiguration : IEntityTypeConfiguration<SingleLineDiagram>
{
    public void Configure(EntityTypeBuilder<SingleLineDiagram> builder)
    {
        builder.ToTable("SingleLineDiagrams", schema: "dbo");
        builder.HasKey(x => x.SingleLineDiagramId);
        builder.Property(x => x.FileUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.SingleLineDiagrams)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
