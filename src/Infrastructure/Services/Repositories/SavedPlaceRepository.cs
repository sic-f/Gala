using Core.Dto;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class SavedPlaceRepository(ApplicationDbContext context) : ISavedPlaceRepository
{
    private SavedPlace _savedPlace = default!;

    public (IEnumerable<Place>, IList<SavedPlace>) FetchSavedPlaces(DateTimeOffset offset, int limit, Guid broId)
    {
        IList<SavedPlace> savedPlaces = [.. context.SavedPlace.AsNoTracking()
            .Where(p => p.CreatedAt < offset)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .Where(sp => sp.SaverId == broId)];

        IList<Guid> savedPlaceIds = [.. savedPlaces.Select(sianca => sianca.SaveableId)];

        IEnumerable<Place> places = context.Place.Include(p => p.ProfilePictures)
            .Where(p => savedPlaceIds.Contains(p.Id))
            .ToList()
            .OrderBy(p => savedPlaceIds.IndexOf(p.Id));

        return (places, savedPlaces);
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
