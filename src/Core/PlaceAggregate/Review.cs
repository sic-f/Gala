using Core.Dto;

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

    public void SetDetails(ReviewDetails details, Guid broId)
    {
        Content = details.Content;
        PlaceId = details.PlaceId;
        ReviewerId = broId;
    }
}
