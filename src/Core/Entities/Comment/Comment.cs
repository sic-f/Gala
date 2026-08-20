namespace Core.Entities.Comment;

public class Comment
{
    public Guid Id { get; set; }
    public string Body { get; set; } = null!;
    public Guid CommentableId { get; set; }
    public Guid CommenterId { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset LastModified { get; set; }
}
