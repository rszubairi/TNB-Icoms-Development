using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects", schema: "config");
        builder.HasKey(x => x.ProjectId);
        builder.Property(x => x.TpCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProjectSuffix).IsRequired().HasMaxLength(150);
        builder.Property(x => x.ProjectName).IsRequired().HasMaxLength(210);
        builder.HasIndex(x => x.TpCode).IsUnique();

        builder.HasOne(x => x.Zone)
            .WithMany()
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
