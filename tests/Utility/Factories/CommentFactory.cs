using Core.Entities.Comment;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Utility.Factories;

public class CommentFactory
{
    public static async Task CreateAsync(IEnumerable<PlacePictureComment> comments, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.PlacePictureComment.AddRangeAsync(comments);
        await context.SaveChangesAsync();
    }
}
