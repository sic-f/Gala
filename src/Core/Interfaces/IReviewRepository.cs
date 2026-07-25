using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IReviewRepository
{
    IQueryable<Review> FetchPaginated(int offset, int limit, Guid placeId);
    Task CreateReviewAsync(Review review);
    Task<Review> FetchAsync(Guid id);
    Task<Review> NoTrackingFetchAsync(Guid id);
    Task ModifyAsync();
    Task DeleteAsync(Review review);
    Task<int> GetReviewsCountAsync(Guid placeId);
}
