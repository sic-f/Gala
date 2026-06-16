namespace Ui.Dto;

public class Place
{
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

    public record PlaceErrors(string Name, string Address);
}
