using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class DropdownValueConfiguration : IEntityTypeConfiguration<DropdownValue>
{
    public void Configure(EntityTypeBuilder<DropdownValue> builder)
    {
        builder.ToTable("DropdownValues", schema: "config");
        builder.HasKey(x => x.DropdownValueId);
        builder.Property(x => x.CategoryCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ValueCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ValueLabel).IsRequired().HasMaxLength(200);
    }
}
