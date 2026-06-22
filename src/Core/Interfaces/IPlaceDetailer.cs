namespace Core.Interfaces;

public interface IPlaceDetailer
{
    string Name { get; set; }
    string? Description { get; set; }
    string? Information { get; set; }
    string Address { get; set; }
    string? Mobile { get; set; }
    string? Landline { get; set; }
    string? Email { get; set; }
    string? Website { get; set; }
    string? Facebook { get; set; }
    string? Instagram { get; set; }
}