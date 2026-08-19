using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class TransmissionLineStationConfiguration : IEntityTypeConfiguration<TransmissionLineStation>
{
    public void Configure(EntityTypeBuilder<TransmissionLineStation> builder)
    {
        builder.ToTable("TransmissionLineStations", schema: "config");
        builder.HasKey(x => x.TransmissionLineStationId);

        builder.HasOne(x => x.TransmissionLine)
            .WithMany(x => x.Stations)
            .HasForeignKey(x => x.TransmissionLineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.GeneratedEquipment)
            .WithMany()
            .HasForeignKey(x => x.GeneratedEquipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
