using Core.Entities;
using Core.PlaceAggregate;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

public class BroConfiguration : IEntityTypeConfiguration<Bro>
{
    public void Configure(EntityTypeBuilder<Bro> builder)
    {
        builder.HasMany<RefreshToken>()
               .WithOne()
               .HasForeignKey(rt => rt.BroId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<Place>()
               .WithOne()
               .HasForeignKey(p => p.BroId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
