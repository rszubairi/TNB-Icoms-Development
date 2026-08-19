using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class CommissioningMemoConfiguration : IEntityTypeConfiguration<CommissioningMemo>
{
    public void Configure(EntityTypeBuilder<CommissioningMemo> builder)
    {
        builder.ToTable("CommissioningMemos", schema: "dbo");
        builder.HasKey(x => x.CommissioningMemoId);
        builder.Property(x => x.MemoNo).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30);

        builder.HasOne(x => x.Outage)
            .WithMany(x => x.CommissioningMemos)
            .HasForeignKey(x => x.OutageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
