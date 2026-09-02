using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Utility.Factories;

public class SavedReviewFactory
{
    public static async Task<SavedReview> CreateAsync(SavedReview savedReview, IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        SavedReview entity = context.SavedReview.Add(savedReview).Entity;
        await context.SaveChangesAsync();

        return entity;
    }

    public static async Task<IEnumerable<SavedReview>> CreateAsync(IEnumerable<SavedReview> savedReviews, IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.SavedReview.AddRangeAsync(savedReviews);
        await context.SaveChangesAsync();

        return savedReviews;
    }
}
