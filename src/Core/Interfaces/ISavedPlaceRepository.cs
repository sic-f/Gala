using Core.Dto;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Core.Interfaces;

public interface ISavedPlaceRepository
{
    IEnumerable<Place> FetchSavedPlaces(Guid saverId);
    Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace);
    Task<Guid?> GetSavedPlaceId(Guid placeId, Guid broId);
}
