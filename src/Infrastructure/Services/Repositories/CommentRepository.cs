using Core.Dto;
using Core.Entities.Comment;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class CommentRepository(ApplicationDbContext context) : ICommentRepository
{
    public IEnumerable<CommentDto> GetPlacePictureComments(Guid placePictureId, DateTimeOffset offset, int limit)
    {
        return PlacePictureComments(placePictureId, offset, limit).AsEnumerable().Join(
            context.Users.AsNoTracking(),
            placePictureComment => placePictureComment.CommenterId,
            bro => bro.Id,
            (placePictureComment, bro) => new CommentDto
            (
                Id: placePictureComment.Id,
                Body: placePictureComment.Body,
                CommenterName: bro.Name(),
                CreatedAt: placePictureComment.CreatedAt,
                LastModified: placePictureComment.LastModified,
                CommenterAvatar: bro.Avatar?.Small
            ));
    }

    public async Task WriteCommentAsync(PlacePictureComment comment)
    {
        await context.PlacePictureComment.AddAsync(comment);
        await context.SaveChangesAsync();
    }

    private IQueryable<PlacePictureComment> PlacePictureComments(Guid placePictureId, DateTimeOffset offset, int limit)
    {
        return context.PlacePictureComment.AsNoTracking()
            .Where(placePictureComment => placePictureComment.CommentableId.Equals(placePictureId))
            .Where(placePictureComment => placePictureComment.CreatedAt < offset)
            .OrderByDescending(placePictureComment => placePictureComment.CreatedAt)
            .Take(limit);
    }
}
