using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class GcuAcknowledgementConfiguration : IEntityTypeConfiguration<GcuAcknowledgement>
{
    public void Configure(EntityTypeBuilder<GcuAcknowledgement> builder)
    {
        builder.ToTable("GcuAcknowledgements", schema: "dbo");
        builder.HasKey(x => x.GcuAcknowledgementId);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.GcuAcknowledgements)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
