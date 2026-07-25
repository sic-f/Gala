using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class SavedRepository(ApplicationDbContext context) : ISavedRepository
{
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

    public async Task<Saved> CreateSavedAsync(Saved newSaved)
    {
        Saved saved = context.Add(newSaved).Entity;
        await context.SaveChangesAsync();

        return saved;
    }

    public async Task<Guid?> GetSavedPlaceId(Guid placeId, Guid broId)
    {
        return (await context.SavedPlace.AsNoTracking().FirstOrDefaultAsync<SavedPlace>(sp =>
            sp.SaveableId == placeId && sp.SaverId == broId))?.Id;
    }

    public async Task<SavedReview> CreateSavedReviewAsync(SavedReview savedReview)
    {
        SavedReview createdSavedReview = context.SavedReview.Add(savedReview).Entity;
        await context.SaveChangesAsync();

        return createdSavedReview;
    }
}
