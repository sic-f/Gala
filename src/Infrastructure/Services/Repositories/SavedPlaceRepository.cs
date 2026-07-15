using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class SavedPlaceRepository(ApplicationDbContext context) : ISavedPlaceRepository
{
    private SavedPlace _savedPlace = default!;

    public IEnumerable<Place> FetchSavedPlaces(DateTimeOffset offset, int limit, Guid broId)
    {
        IList<Guid> savedPlaceIds = [.. context.SavedPlace.AsNoTracking()
            .Where(p => p.CreatedAt < offset)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .Where(sp => sp.SaverId == broId)
            .Select(sp => sp.SaveableId)];

        return context.Place.Include(p => p.ProfilePictures)
            .Where(p => savedPlaceIds.Contains(p.Id))
            .ToList()
            .OrderBy(p => savedPlaceIds.IndexOf(p.Id));
    }

    public async Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace)
    {
        _savedPlace = context.SavedPlace.Add(savedPlace).Entity;
        await context.SaveChangesAsync();

        return _savedPlace;
    }

    public async Task<Guid?> GetSavedPlaceId(Guid placeId, Guid broId)
    {
        return (await context.SavedPlace.AsNoTracking().FirstOrDefaultAsync<SavedPlace>(sp =>
            sp.SaveableId == placeId && sp.SaverId == broId))?.Id;
    }
}
