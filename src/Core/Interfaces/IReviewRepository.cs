using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IReviewRepository
{
    Task CreateReviewAsync(Review review);
}
