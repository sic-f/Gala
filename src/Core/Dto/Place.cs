namespace Core.Dto;

public record PlaceDetailsHomePage
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Information { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string? Landline { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public Guid BroId { get; set; }
    public Guid? BookmarkId { get; set; }
    public PlacePictureLean? Picture { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
};
public record PlaceDetails(Guid Id,
                           string Name,
                           string? Description,
                           string? Information,
                           string Address,
                           string? Mobile,
                           string? Landline,
                           string? Email,
                           string? Website,
                           string? Facebook,
                           string? Instagram,
                           Guid BroId,
                           Guid? BookmarkId,
                           PlacePictureLean? Picture,
                           IEnumerable<PlacePictureLean> Pictures);