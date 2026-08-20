using Core.Entities.Comment;

namespace Core.Interfaces;

public interface ICommentRepository
{
    Task WriteCommentAsync(PlacePictureComment comment);
}
