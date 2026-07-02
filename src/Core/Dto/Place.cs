namespace Core.Dto;

public record PlaceDetailsHomePage(Guid Id,
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
                           PlaceGeneralPictureDetails? Picture,
                           IEnumerable<PlaceGeneralPictureDetails> Pictures,
                           DateTimeOffset CreatedAt);
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
                           PlaceGeneralPictureDetails? Picture,
                           IEnumerable<PlaceGeneralPictureDetails> Pictures);

public record PlaceGeneralPictureDetails(Guid Id, string? Caption, string Normal, string Small, Guid PlaceId);

