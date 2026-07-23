using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IReviewRepository
{
    Task CreateReviewAsync(Review review);
    Task<Review> FetchAsync(Guid id);
    Task<Review> NoTrackingFetchAsync(Guid id);
    Task ModifyAsync();
    Task DeleteAsync(Review review);
}
