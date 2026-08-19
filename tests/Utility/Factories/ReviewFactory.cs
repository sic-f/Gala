using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Utility.Factories;

public class ReviewFactory
{
    public static async Task<Review> CreateAsync(Review review, IServiceProvider services)
    {
        var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var entity = context.Review.Add(review).Entity;
        await context.SaveChangesAsync();

        return entity;
    }

    public static async Task<IEnumerable<Review>> CreateAsync(IEnumerable<Review> reviews, IServiceProvider services)
    {
        var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Review.AddRange(reviews);
        await context.SaveChangesAsync();

        return reviews;
    }
}
