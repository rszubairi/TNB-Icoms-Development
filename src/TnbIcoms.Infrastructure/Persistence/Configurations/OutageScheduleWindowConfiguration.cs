using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OutageScheduleWindowConfiguration : IEntityTypeConfiguration<OutageScheduleWindow>
{
    public void Configure(EntityTypeBuilder<OutageScheduleWindow> builder)
    {
        builder.ToTable("OutageScheduleWindows", schema: "config");
        builder.HasKey(x => x.OutageScheduleWindowId);
        builder.Property(x => x.WorkTypeCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.OutageTypeCode).IsRequired().HasMaxLength(30);
        builder.HasIndex(x => new { x.WorkTypeCode, x.OutageTypeCode, x.Month }).IsUnique();
    }
}
