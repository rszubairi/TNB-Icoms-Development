using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OutageTypeRuleConfiguration : IEntityTypeConfiguration<OutageTypeRule>
{
    public void Configure(EntityTypeBuilder<OutageTypeRule> builder)
    {
        builder.ToTable("OutageTypeRules", schema: "config");
        builder.HasKey(x => x.OutageTypeRuleId);
        builder.Property(x => x.OutageTypeCode).IsRequired().HasMaxLength(30);
    }
}
