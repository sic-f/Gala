using Core.Entities.Comment;
using Core.PlaceAggregate;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(comment => comment.Body)
            .HasMaxLength(8000)
            .IsRequired();

        builder.Property(comment => comment.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
        builder.Property(comment => comment.LastModified).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");

        builder.HasOne<Bro>()
            .WithMany()
            .HasForeignKey(comment => comment.CommenterId)
            .IsRequired();

        builder.HasOne<PlacePicture>()
            .WithMany(placePicture => placePicture.Comments)
            .HasForeignKey(comment => comment.CommentableId)
            .IsRequired();

        builder.HasIndex(comment => comment.CommentableId);
    }
}
