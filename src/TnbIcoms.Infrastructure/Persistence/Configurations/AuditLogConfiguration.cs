using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Audit;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", schema: "audit");
        builder.HasKey(x => x.AuditLogId);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.EntityName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.EntityId).HasMaxLength(50);
        builder.Property(x => x.IpAddress).HasMaxLength(50);
    }
}
