using Core.SavedAggregate;

namespace Core.Interfaces;

public interface ISavedPlaceRepository
{
    IEnumerable<SavedPlace> FetchSavedPlaces(Guid saverId);
    Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace);
    void DeleteSavedPlace(Guid id);
    Task<Guid?> GetSavedPlaceId(Guid placeId, Guid broId);
}
