using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(rt => rt.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
        builder.Property(rt => rt.Token).HasMaxLength(128);
        builder.Property(rt => rt.ReplacedByToken).HasMaxLength(128);
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => rt.ReplacedByToken).IsUnique();
        builder.HasIndex(rt => new { rt.Expires, rt.Revoked });
        builder.HasIndex(rt => rt.BroId).IsUnique(false);
    }
}
