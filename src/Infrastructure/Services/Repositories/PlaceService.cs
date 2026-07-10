using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class PlaceService(ApplicationDbContext context) : IPlaceService
{
    public async Task<Place> SavePlaceAsync(Place place)
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
}
