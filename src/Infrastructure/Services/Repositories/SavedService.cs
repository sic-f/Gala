using Core.Interfaces;
using Core.SavedAggregate;
using Infrastructure.Data;

namespace Infrastructure.Services.Repositories;

public class SavedService(ApplicationDbContext context) : ISavedPlaceService
{
    private SavedPlace _savedPlace = default!;

    public IEnumerable<SavedPlace> FetchSavedPlaces(Guid saverId)
    {
        throw new NotImplementedException();
    }

    public async Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace)
    {
        _savedPlace = context.SavedPlace.Add(savedPlace).Entity;
        await context.SaveChangesAsync();

        return _savedPlace;
    }

    public void DeleteSavedPlace(Guid id)
    {
        throw new NotImplementedException();
    }
}
