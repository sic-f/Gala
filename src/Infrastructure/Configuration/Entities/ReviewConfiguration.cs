using Core.PlaceAggregate;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

internal class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.Headline)
            .HasMaxLength(75)
            .IsRequired(false);

        builder.HasIndex(r => r.Headline);

        builder.Property(r => r.Content)
            .HasMaxLength(50000)
            .IsRequired();

        builder.Property(r => r.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
        builder.Property(r => r.LastModified).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");

        builder.HasOne<Place>()
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.PlaceId)
            .IsRequired();

        builder.HasOne<Bro>()
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .IsRequired();
    }
}
