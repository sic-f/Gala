using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class PlaceService(ApplicationDbContext context) : IPlaceService
{
    public async Task<Place> CreatePlaceAsync(Place place)
    {
        Place entity = (await context.Place.AddAsync(place)).Entity;

        try
        {
            await context.SaveChangesAsync();

            return entity;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
    }

    public async Task<Place> FetchPlaceWithPictureAsync(Guid id)
    {
        return await context.Place.Include(p => p.ProfilePictures).FirstAsync<Place>(p => p.Id == id);
    }

    public async Task ModifyPlaceAsync(Place place)
    {
        await context.SaveChangesAsync();
    }
}
