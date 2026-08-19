using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Audit;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("ErrorLogs", schema: "audit");
        builder.HasKey(x => x.ErrorLogId);
        builder.Property(x => x.Source).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Severity).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Message).IsRequired();
        builder.HasIndex(x => x.OccurredAt);
    }
}
