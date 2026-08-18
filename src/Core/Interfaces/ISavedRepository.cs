using Core.Entities;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Core.Interfaces;

public interface ISavedRepository
{
    (IEnumerable<Place>,
     IEnumerable<Saved>,
     IEnumerable<Review>,
     IEnumerable<Saved>) FetchSaveds(DateTimeOffset offset, int limit, Guid broId);
    Task<Saved> FetchSavedAsync(Guid savedId);
    IEnumerable<Core.Dto.SavedDto> GetSavedPlaces(DateTimeOffset offset, int limit, Guid broId);
    (IOrderedEnumerable<Review>, IQueryable<SavedReview>) GetSavedReviews(DateTimeOffset offset, int limit, Guid broId);
    Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace);
    Task<Guid?> GetSavedId(Guid placeId, Guid broId);
    Task<SavedReview> CreateSavedReviewAsync(SavedReview savedReview);
    Task<AccountAvatar?> GetReviewerAvatarAsync(Guid reviewId);
    Task DeleteSavedAsync(Saved saved);
}
