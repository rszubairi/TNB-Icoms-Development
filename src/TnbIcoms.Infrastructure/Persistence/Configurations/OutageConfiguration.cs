using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OutageConfiguration : IEntityTypeConfiguration<Outage>
{
    public void Configure(EntityTypeBuilder<Outage> builder)
    {
        builder.ToTable("Outages", schema: "dbo");
        builder.HasKey(x => x.OutageId);
        builder.Property(x => x.OutageNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.OutageTypeCode).IsRequired().HasMaxLength(30);
        builder.Property(x => x.OutageClass).IsRequired().HasMaxLength(30);
        builder.Property(x => x.WorkTypeCode).IsRequired().HasMaxLength(30);
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.RequestorStatus).IsRequired().HasMaxLength(50);
        builder.Property(x => x.GnmStatus).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.OutageNumber).IsUnique();

        builder.HasOne(x => x.Zone)
            .WithMany()
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.VoltageLevel)
            .WithMany()
            .HasForeignKey(x => x.VoltageLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PrimaryEquipment)
            .WithMany()
            .HasForeignKey(x => x.PrimaryEquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
