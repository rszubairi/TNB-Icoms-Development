using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Audit;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class SavedReportFilterConfiguration : IEntityTypeConfiguration<SavedReportFilter>
{
    public void Configure(EntityTypeBuilder<SavedReportFilter> builder)
    {
        builder.ToTable("SavedReportFilters", schema: "audit");
        builder.HasKey(x => x.SavedReportFilterId);
        builder.Property(x => x.FilterName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.ReportCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.FilterJson).IsRequired();
    }
}
