using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.ToTable("Organisations", schema: "config");
        builder.HasKey(x => x.OrganisationId);
        builder.Property(x => x.OrganisationName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.OrganisationCode).HasMaxLength(30);
    }
}
