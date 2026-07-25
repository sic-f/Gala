using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public IQueryable<Review> FetchPaginated(int offset, int limit, Guid placeId)
    {
        return context.Review.AsNoTracking()
                             .Where(r => r.PlaceId == placeId)
                             .OrderByDescending(r => r.CreatedAt)
                             .Skip(offset)
                             .Take(limit);
    }

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

    public async Task<int> GetReviewsCountAsync(Guid placeId)
    {
        return await context.Review.Where(r => r.PlaceId == placeId).CountAsync();
    }
}