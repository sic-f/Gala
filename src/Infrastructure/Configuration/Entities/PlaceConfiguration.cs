using Core.PlaceAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
    public void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(75);

        builder.HasIndex(p => p.Name);

        builder.Property(p => p.Description)
               .HasMaxLength(155);

        builder.Property(p => p.Information)
               .HasMaxLength(50000);

        builder.Property(p => p.Address)
               .IsRequired()
               .HasMaxLength(95);

        builder.HasIndex(p => p.Address);

        builder.Property(p => p.Mobile)
               .HasMaxLength(15);

        builder.Property(p => p.Landline)
               .HasMaxLength(15);

        builder.Property(p => p.Email)
               .HasMaxLength(254);

        builder.HasIndex(p => p.Email);

        builder.Property(p => p.Website)
               .HasMaxLength(2083);

        builder.Property(p => p.Facebook)
               .HasMaxLength(2083);

        builder.Property(p => p.Instagram)
               .HasMaxLength(30);

        builder.HasIndex(p => p.Instagram);

        builder.HasIndex(p => p.BroId);

        builder.Property(p => p.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");

        builder.HasMany<PlaceProfilePicture>(p => p.ProfilePictures)
               .WithOne()
               .HasForeignKey(ppp => ppp.PlaceId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<PlaceExtraPicture>(p => p.Pictures)
               .WithOne()
               .HasForeignKey(pep => pep.PlaceId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}