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

    public async Task<Review> NoTrackingFetchAsync(Guid id)
    {
        return await context.Review.AsNoTracking().FirstAsync(r => r.Id == id);
    }

    public async Task ModifyAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        context.Review.Remove(review);
        await context.SaveChangesAsync();
    }
}