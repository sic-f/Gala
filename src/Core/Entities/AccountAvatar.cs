namespace Core.Entities;

public class AccountAvatar
{
    public const int DisplayWidth = 854;
    public const int DisplayHeight = 480;
    public const int ThumbnailWidth = 20;
    public const int ThumbnailHeight = 20;

    public Guid Id { get; set; }
    public string Normal { get; set; } = string.Empty;
    public string Small { get; set; } = string.Empty;
    public Guid BroId { get; set; }
}
