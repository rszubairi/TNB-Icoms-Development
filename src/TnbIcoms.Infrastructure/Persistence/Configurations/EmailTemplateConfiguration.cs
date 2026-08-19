using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.ToTable("EmailTemplates", schema: "config");
        builder.HasKey(x => x.EmailTemplateId);
        builder.Property(x => x.TemplateCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(500);
        builder.Property(x => x.BodyHtml).IsRequired();
        builder.Property(x => x.AvailableTags).HasMaxLength(500);
        builder.HasIndex(x => x.TemplateCode).IsUnique();
    }
}
