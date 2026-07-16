using Core.Dto;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Core.Interfaces;

public interface ISavedPlaceRepository
{
    (IEnumerable<Place>, IList<SavedPlace>) FetchSavedPlaces(DateTimeOffset offset, int limit, Guid broId);
    Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace);
    Task<Guid?> GetSavedPlaceId(Guid placeId, Guid broId);
}
