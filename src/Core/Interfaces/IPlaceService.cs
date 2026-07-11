using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IPlaceService
{
    Task<Place> CreatePlaceAsync(Place place);
    Task<Place> FetchPlaceAsync(Guid id);
    Task ModifyPlaceAsync();
}
