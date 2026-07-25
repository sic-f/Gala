using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Core.Interfaces;

public interface ISavedRepository
{
    (IEnumerable<Place>, IList<SavedPlace>) FetchSavedPlaces(DateTimeOffset offset, int limit, Guid broId);
    Task<Saved> CreateSavedAsync(Saved newSaved);
    Task<Guid?> GetSavedPlaceId(Guid placeId, Guid broId);
    Task<SavedReview> CreateSavedReviewAsync(SavedReview savedReview);
}
