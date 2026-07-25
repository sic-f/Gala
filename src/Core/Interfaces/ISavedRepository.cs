using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Core.Interfaces;

public interface ISavedRepository
{
    (IEnumerable<Place>, IList<SavedPlace>) FetchSavedPlaces(DateTimeOffset offset, int limit, Guid broId);
    Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace);
    Task<Guid?> GetSavedId(Guid placeId, Guid broId);
    Task<SavedReview> CreateSavedReviewAsync(SavedReview savedReview);
}
