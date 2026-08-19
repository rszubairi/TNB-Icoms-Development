using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TnbIcoms.Domain.Entities.Auth;

namespace TnbIcoms.Infrastructure.Persistence.Configurations;

public class RoleTransferRequestConfiguration : IEntityTypeConfiguration<RoleTransferRequest>
{
    public void Configure(EntityTypeBuilder<RoleTransferRequest> builder)
    {
        builder.ToTable("RoleTransferRequests", schema: "auth");
        builder.HasKey(x => x.RoleTransferRequestId);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FromRole)
            .WithMany()
            .HasForeignKey(x => x.FromRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToRole)
            .WithMany()
            .HasForeignKey(x => x.ToRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
