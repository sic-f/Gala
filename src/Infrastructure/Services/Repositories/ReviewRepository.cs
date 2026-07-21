using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public async Task CreateReviewAsync(Review review)
    {
        context.Review.Add(review);
        await context.SaveChangesAsync();
    }

    public async Task<Review> FetchAsync(Guid id)
    {
        return await context.Review.FirstAsync(r => r.Id == id);
    }

    public async Task ModifyAsync()
    {
        await context.SaveChangesAsync();
    }
}