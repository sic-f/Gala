using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Utility.Dto;

public record PlaceDetails : IPlaceDetailer
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Information { get; set; }
    public string Address { get; set; } = null!;
    public string? Mobile { get; set; }
    public string? Landline { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public IFormFile? Picture { get; set; }
    public IFormFileCollection? Pictures { get; set; }
}
