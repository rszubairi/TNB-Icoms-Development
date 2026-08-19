using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class ChangeRequestConfiguration : IEntityTypeConfiguration<ChangeRequest>
{
    public void Configure(EntityTypeBuilder<ChangeRequest> builder)
    {
        builder.ToTable("ChangeRequests", schema: "dbo");
        builder.HasKey(x => x.ChangeRequestId);
        builder.Property(x => x.FieldName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.ChangeRequests)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
