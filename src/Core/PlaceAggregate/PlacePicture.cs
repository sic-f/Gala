using Core.Entities.Comment;
using Core.Interfaces;

namespace Core.PlaceAggregate;

public class PlacePicture : IImageDeletable, IImageTypeSegmentable
{
    public static string ImageTypeSegment { get; } = "Images\\Place";

    public Guid Id { get; set; }
    public string? Caption { get; set; }
    public string Normal { get; set; } = string.Empty;
    public string Small { get; set; } = string.Empty;
    public string NormalLocal { get; set; } = string.Empty;
    public string SmallLocal { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
    public Guid? UploaderId { get; set; }
    public ICollection<Comment> Comments { get; } = [];
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset LastModified { get; set; }
}
