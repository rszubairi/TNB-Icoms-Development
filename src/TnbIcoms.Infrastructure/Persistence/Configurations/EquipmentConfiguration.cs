using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment", schema: "config");
        builder.HasKey(x => x.EquipmentId);
        builder.Property(x => x.EquipmentName).IsRequired().HasMaxLength(250);
        builder.Property(x => x.EquipmentCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ShortName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LineFilterType).HasMaxLength(20);
        builder.HasIndex(x => x.EquipmentCode).IsUnique();

        builder.HasOne(x => x.EquipmentType)
            .WithMany()
            .HasForeignKey(x => x.EquipmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.VoltageLevel)
            .WithMany()
            .HasForeignKey(x => x.VoltageLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Zone)
            .WithMany()
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
