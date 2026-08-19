using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OutagePicConfiguration : IEntityTypeConfiguration<OutagePic>
{
    public void Configure(EntityTypeBuilder<OutagePic> builder)
    {
        builder.ToTable("OutagePics", schema: "dbo");
        builder.HasKey(x => x.OutagePicId);
        builder.Property(x => x.PicName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.PicEmail).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PicPhone).HasMaxLength(30);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.Pics)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
