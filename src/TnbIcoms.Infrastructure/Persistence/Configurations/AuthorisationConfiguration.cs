using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class AuthorisationConfiguration : IEntityTypeConfiguration<Authorisation>
{
    public void Configure(EntityTypeBuilder<Authorisation> builder)
    {
        builder.ToTable("Authorisations", schema: "dbo");
        builder.HasKey(x => x.AuthorisationId);
        builder.Property(x => x.AuthorisationNo).IsRequired().HasMaxLength(50);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.Authorisations)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
