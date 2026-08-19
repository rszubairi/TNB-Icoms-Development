using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class VoltageLevelConfiguration : IEntityTypeConfiguration<VoltageLevel>
{
    public void Configure(EntityTypeBuilder<VoltageLevel> builder)
    {
        builder.ToTable("VoltageLevels", schema: "config");
        builder.HasKey(x => x.VoltageLevelId);
        builder.Property(x => x.LevelName).IsRequired().HasMaxLength(30);
    }
}
