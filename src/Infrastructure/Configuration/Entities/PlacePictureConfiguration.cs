using Core.PlaceAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

public class PlacePictureConfiguration : IEntityTypeConfiguration<PlacePicture>
{
    public void Configure(EntityTypeBuilder<PlacePicture> builder)
    {
        builder.Property(pp => pp.Caption)
               .HasMaxLength(155)
               .IsRequired(false);

        builder.Property(pp => pp.Normal)
               .HasMaxLength(2083)
               .IsRequired();

        builder.Property(pp => pp.Small)
               .HasMaxLength(2083)
               .IsRequired();

        builder.Property(pp => pp.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
        builder.Property(pp => pp.LastModified).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
    }
}
