using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;

namespace Infrastructure.Services.Repositories;

public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public async Task CreateReviewAsync(Review review)
    {
        context.Review.Add(review);
        await context.SaveChangesAsync();
    }
}
