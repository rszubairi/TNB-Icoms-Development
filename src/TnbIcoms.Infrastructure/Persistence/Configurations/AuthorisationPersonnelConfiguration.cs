using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class AuthorisationPersonnelConfiguration : IEntityTypeConfiguration<AuthorisationPersonnel>
{
    public void Configure(EntityTypeBuilder<AuthorisationPersonnel> builder)
    {
        builder.ToTable("AuthorisationPersonnel", schema: "config");
        builder.HasKey(x => x.AuthorisationPersonnelId);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.Property(x => x.StaffId).HasMaxLength(50);

        builder.HasOne(x => x.Zone)
            .WithMany()
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
