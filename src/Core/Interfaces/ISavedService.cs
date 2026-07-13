using Core.SavedAggregate;

namespace Core.Interfaces;

public interface ISavedPlaceService
{
    IEnumerable<SavedPlace> FetchSavedPlaces(Guid saverId);
    Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace);
    void DeleteSavedPlace(Guid id);
    Task<bool> AnySavedPlaceAsync(Guid placeId, Guid broId);
}
