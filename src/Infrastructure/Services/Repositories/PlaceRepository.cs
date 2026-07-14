using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class PlaceRepository(ApplicationDbContext context) : IPlaceRepository
{
    public IEnumerable<Place> FetchPlacePaginatedWithPicture(DateTimeOffset offset, int limit)
    {
        return context.Place.AsNoTracking()
                            .Include(p => p.ProfilePictures)
                            .Where(p => p.CreatedAt < offset)
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(limit);
    }

    public async Task<Place> CreatePlaceAsync(Place place)
    {
        Place entity = (await context.Place.AddAsync(place)).Entity;

        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<Place> FetchPlaceWithPictureAsync(Guid id)
    {
        return await context.Place.Include(p => p.ProfilePictures).FirstAsync<Place>(p => p.Id == id);
    }

    public Place FetchPlaceComplete(Guid id)
    {
        return context.Place.AsNoTracking()
                            .Include(p => p.ProfilePictures)
                            .Include(p => p.Pictures)
                            .First(p => p.Id == id);
    }

    public async Task<Place> FetchPlaceAsync(Guid id)
    {
        return await context.Place.AsNoTracking().FirstAsync(p => p.Id == id);
    }

    public IEnumerable<PlacePicture> FetchPlacePictureAndPictures(Guid id)
    {
        return context.PlacePicture.AsNoTracking().Where(pp => pp.PlaceId == id);
    }

    public async Task ModifyPlaceAsync(Place place)
    {
        await context.SaveChangesAsync();
    }

    public async Task DeletePlaceAsync(Place place)
    {
        context.Place.Remove(place);
        await context.SaveChangesAsync();
    }
}
