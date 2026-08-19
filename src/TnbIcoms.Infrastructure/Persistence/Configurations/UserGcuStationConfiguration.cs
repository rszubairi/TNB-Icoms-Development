using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Auth;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class UserGcuStationConfiguration : IEntityTypeConfiguration<UserGcuStation>
{
    public void Configure(EntityTypeBuilder<UserGcuStation> builder)
    {
        builder.ToTable("UserGcuStations", schema: "auth");
        builder.HasKey(x => x.UserGcuStationId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.GcuStations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
