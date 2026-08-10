using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Utility.Factories;

public class SavedReviewFactory
{
    public static async Task<SavedReview> CreateAsync(SavedReview savedReview, IServiceProvider services)
    {
        var context = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

        SavedReview entity = context.SavedReview.Add(savedReview).Entity;
        await context.SaveChangesAsync();

        return entity;
    }
}
