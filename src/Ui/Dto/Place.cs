using Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Ui.Dto;

public record PlaceDetails : IPlaceSetter
{
    [Required(ErrorMessage = "Name of place is required")]
    [MaxLength(75, ErrorMessage = "Exceeds 75 characters")]
    public string Name { get; set; } = null!;
    [MaxLength(155, ErrorMessage = "Exceeds 155 characters")]
    public string? Description { get; set; }
    [MaxLength(50000, ErrorMessage = "Exceeds 50,000 characters")]
    public string? Information { get; set; }
    [Required(ErrorMessage = "Address is required")]
    [MaxLength(95, ErrorMessage = "Exceeds 95 characters")]
    public string Address { get; set; } = null!;
    [MaxLength(15, ErrorMessage = "Exceeds 15 digits")]
    public string? Mobile { get; set; }
    [MaxLength(15, ErrorMessage = "Exceeds 15 digits")]
    public string? Landline { get; set; }
    [MaxLength(254, ErrorMessage = "Exceeds 254 characters")]
    public string? Email { get; set; }
    [MaxLength(2083, ErrorMessage = "Exceeds 2,083 characters")]
    public string? Website { get; set; }
    [MaxLength(2083, ErrorMessage = "Exceeds 2,083 characters")]
    public string? Facebook { get; set; }
    [MaxLength(30, ErrorMessage = "Exceeds 30 characters")]
    public string? Instagram { get; set; }
    public IFormFile? Picture { get; set; }
    public IFormFileCollection? Pictures { get; set; }
}