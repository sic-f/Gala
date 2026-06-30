using Core.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

public class AccountAvatarConfiguration : IEntityTypeConfiguration<AccountAvatar>
{
    public void Configure(EntityTypeBuilder<AccountAvatar> builder)
    {
        builder.Property(aa => aa.Normal)
               .HasMaxLength(2083);

        builder.Property(aa => aa.Small)
               .HasMaxLength(2083);

        builder.Property(aa => aa.CreatedAt)
               .HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
    }
}
