using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IPlaceService
{
    Task<Place> SavePlaceAsync(Place place);
}
