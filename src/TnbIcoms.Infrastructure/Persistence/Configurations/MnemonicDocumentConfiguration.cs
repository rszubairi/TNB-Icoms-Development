using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class MnemonicDocumentConfiguration : IEntityTypeConfiguration<MnemonicDocument>
{
    public void Configure(EntityTypeBuilder<MnemonicDocument> builder)
    {
        builder.ToTable("MnemonicDocuments", schema: "config");
        builder.HasKey(x => x.MnemonicDocumentId);
        builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(260);
        builder.Property(x => x.StoredFileName).IsRequired().HasMaxLength(260);

        builder.HasOne(x => x.UploadedByUser)
            .WithMany()
            .HasForeignKey(x => x.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
