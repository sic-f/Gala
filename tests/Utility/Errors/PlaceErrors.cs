using Core.PlaceAggregate;
using Utility.Scopes;
namespace Utility.Errors;

public class PlaceErrors(PlaceScope app) : BaseErrors(app)
{
    public string? Name => Errors?.GetProperty(nameof(Place.Name))[0].GetString();
    public string? Address => Errors?.GetProperty(nameof(Place.Address))[0].GetString();
    public string? Description => Errors?.GetProperty(nameof(Place.Description))[0].GetString();
    public string? Information => Errors?.GetProperty(nameof(Place.Information))[0].GetString();
    public string? Mobile => Errors?.GetProperty(nameof(Place.Mobile))[0].GetString();
    public string? Landline => Errors?.GetProperty(nameof(Place.Landline))[0].GetString();
    public string? Email => Errors?.GetProperty(nameof(Place.Email))[0].GetString();
    public string? Website => Errors?.GetProperty(nameof(Place.Website))[0].GetString();
    public string? Facebook => Errors?.GetProperty(nameof(Place.Facebook))[0].GetString();
    public string? Instagram => Errors?.GetProperty(nameof(Place.Instagram))[0].GetString();
}