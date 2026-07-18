using Core.Dto;
using Core.Interfaces;

namespace Context;

public class Review(IReviewRepository repository)
{
    public async Task<Core.PlaceAggregate.Review> WriteReviewAsync(ReviewDetails details, Guid broId)
    {
        var review = new Core.PlaceAggregate.Review();

        review.SetDetails(details, broId);

        await repository.CreateReviewAsync(review);

        return review;
    }
}
