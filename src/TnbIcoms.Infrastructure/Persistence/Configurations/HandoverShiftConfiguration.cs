using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Handover;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class HandoverShiftConfiguration : IEntityTypeConfiguration<HandoverShift>
{
    public void Configure(EntityTypeBuilder<HandoverShift> builder)
    {
        builder.ToTable("HandoverShifts", schema: "handover");
        builder.HasKey(x => x.ShiftId);
        builder.Property(x => x.ShiftType).IsRequired().HasMaxLength(20);

        builder.HasOne(x => x.Zone)
            .WithMany()
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
