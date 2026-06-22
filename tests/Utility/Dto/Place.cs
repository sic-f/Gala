namespace Utility.Dto;

public record PlaceDetails(
string Name,
string Address,
string? Description = null,
string? Information = null,
string? Mobile = null,
string? Landline = null,
string? Email = null,
string? Website = null,
string? Facebook = null,
string? Instagram = null);

public record PlaceAttributes(
string Name,
string Address,
Guid BroId,
string? Description = null,
string? Information = null,
string? Mobile = null,
string? Landline = null,
string? Email = null,
string? Website = null,
string? Facebook = null,
string? Instagram = null);

public record PlaceUpdateAttributes(string Name, string Address);