using Core.Entities.Comment;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Services.Repositories;

public class CommentRepository(ApplicationDbContext context) : ICommentRepository
{
    public async Task WriteCommentAsync(PlacePictureComment comment)
    {
        await context.PlacePictureComment.AddAsync(comment);
        await context.SaveChangesAsync();
    }
}
