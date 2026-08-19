using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class EquipmentTypeConfiguration : IEntityTypeConfiguration<EquipmentType>
{
    public void Configure(EntityTypeBuilder<EquipmentType> builder)
    {
        builder.ToTable("EquipmentTypes", schema: "config");
        builder.HasKey(x => x.EquipmentTypeId);
        builder.Property(x => x.TypeName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.TypeCode).HasMaxLength(30);
    }
}
