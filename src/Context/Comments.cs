using Core.Dto;
using Core.Entities.Comment;
using Core.Interfaces;

namespace Context;

public class Comments(ICommentRepository repository)
{
    public IEnumerable<CommentDto> GetPlacePictureComments(Guid placePictureId, DateTimeOffset offset, int limit)
    {
        return repository.GetPlacePictureComments(placePictureId, offset, limit);
    }

    public async Task WriteCommentAsync(CommentDetails details, Guid placePictureId, Guid broId)
    {
        PlacePictureComment comment = new();
        SetDetails(details, placePictureId, broId, comment);

        await repository.WriteCommentAsync(comment);
    }

    private static void SetDetails(CommentDetails details, Guid placePictureId, Guid broId, PlacePictureComment comment)
    {
        comment.SetDetails(details, placePictureId, broId);
    }
}
