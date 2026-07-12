using Core.Interfaces;

namespace Core.Entities;

public class AccountAvatar : IImageDeletable, IImageTypeSegmentable
{
    public const int DisplayWidth = 854;
    public const int DisplayHeight = 480;
    public const int ThumbnailWidth = 20;
    public const int ThumbnailHeight = 20;

    public static string ImageTypeSegment { get; } = "Images\\Avatar";

    public Guid Id { get; set; }
    public string Normal { get; set; } = string.Empty;
    public string Small { get; set; } = string.Empty;
    public string NormalLocal { get; set; } = string.Empty;
    public string SmallLocal { get; set; } = string.Empty;
    public Guid BroId { get; set; }
    public DateTimeOffset CreatedAt { get; }
}
