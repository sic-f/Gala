using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Utility.Factories;

public class SavedPlaceFactory
{
    public static async Task<SavedPlace> CreateAsync(Dto.SavedPlace attributes, IServiceProvider services)
    {
        SavedPlace savedPlace = new()
        {
            SaveableId = attributes.SaveableId,
            SaverId = attributes.SaverId,
        };

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        SavedPlace entity = context.SavedPlace.Add(savedPlace).Entity;
        await context.SaveChangesAsync();

        return entity;
    }

    public static async Task<IEnumerable<SavedPlace>> CreateAsync(IEnumerable<SavedPlace> savedPlaces, IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.SavedPlace.AddRange(savedPlaces);

        await context.SaveChangesAsync();

        return savedPlaces;
    }
}
