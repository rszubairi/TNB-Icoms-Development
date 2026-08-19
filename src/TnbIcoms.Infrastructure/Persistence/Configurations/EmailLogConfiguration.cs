using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Audit;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.ToTable("EmailLogs", schema: "audit");
        builder.HasKey(x => x.EmailLogId);
        builder.Property(x => x.TemplateCode).HasMaxLength(100);
        builder.Property(x => x.ToAddress).IsRequired().HasMaxLength(320);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(500);
        builder.Property(x => x.BodyHtml).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasMaxLength(20);
        builder.HasIndex(x => x.SentAt);
    }
}
