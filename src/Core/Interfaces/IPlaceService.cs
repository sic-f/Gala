using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IPlaceService
{
    Task<Place> CreatePlaceAsync(Place place);
    Task<Place> FetchPlaceWithPictureAsync(Guid id);
    Task ModifyPlaceAsync(Place place);
}
