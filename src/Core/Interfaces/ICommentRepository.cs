using Core.Dto;
using Core.Entities.Comment;

namespace Core.Interfaces;

public interface ICommentRepository
{
    IEnumerable<CommentDto> GetPlacePictureComments(Guid placePictureId, DateTimeOffset offset, int limit);
    Task WriteCommentAsync(PlacePictureComment comment);
}
