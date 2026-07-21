using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IReviewRepository
{
    Task CreateReviewAsync(Review review);
    Task<Review> FetchAsync(Guid id);

    Task ModifyAsync();
}
