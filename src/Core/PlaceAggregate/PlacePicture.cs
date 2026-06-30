namespace Core.PlaceAggregate;

public class PlacePicture
{
    public Guid Id { get; set; }
    public string? Caption { get; set; }
    public string Normal { get; set; } = string.Empty;
    public string Small { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
    public Guid? UploaderId { get; set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset LastModified { get; set; }
}
