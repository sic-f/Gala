using Core.Interfaces;
using Core.SavedAggregate;

namespace Context;

public class Bookmark(ISavedPlaceRepository repository)
{
    public async Task<SavedPlace> SavePlaceAsync(Guid placeId, Guid broId)
    {
        SavedPlace savedPlace = new()
        {
            SaveableId = placeId,
            SaverId = broId,
        };

        return await repository.CreateSavedPlaceAsync(savedPlace);
    }
}
