using Core.PlaceAggregate;

namespace Utility.Dto;

public record PlaceAttributes
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? Description { get; set; }
    public string? Information { get; set; }
    public string? Mobile { get; set; }
    public string? Landline { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public required Guid BroId { get; set; }
    public PlaceProfilePicture? Picture { get; set; }
    public IEnumerable<PlaceExtraPicture> Pictures { get; set; } = [];
}