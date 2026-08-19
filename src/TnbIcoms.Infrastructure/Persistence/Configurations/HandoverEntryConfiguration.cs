using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Handover;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class HandoverEntryConfiguration : IEntityTypeConfiguration<HandoverEntry>
{
    public void Configure(EntityTypeBuilder<HandoverEntry> builder)
    {
        builder.ToTable("HandoverEntries", schema: "handover");
        builder.HasKey(x => x.HandoverEntryId);
        builder.Property(x => x.Category).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Description).IsRequired();

        builder.HasOne(x => x.Shift)
            .WithMany(x => x.Entries)
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
