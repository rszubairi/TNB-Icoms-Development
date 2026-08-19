using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class OutageNotifyEmailConfiguration : IEntityTypeConfiguration<OutageNotifyEmail>
{
    public void Configure(EntityTypeBuilder<OutageNotifyEmail> builder)
    {
        builder.ToTable("OutageNotifyEmails", schema: "dbo");
        builder.HasKey(x => x.OutageNotifyEmailId);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.NotifyEmails)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
