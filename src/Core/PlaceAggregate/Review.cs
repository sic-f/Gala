namespace Core.PlaceAggregate;

public class Review
{
    public Guid Id { get; set; }
    public string? Headline { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
    public Guid ReviewerId { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset LastModified { get; set; }
}
