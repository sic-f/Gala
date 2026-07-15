using Core.Dto;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Context;

public class Bookmark(ISavedPlaceRepository repository)
{
    public IEnumerable<PlaceBookmark> FetchBookmarks(Guid broId)
    {
        Place[] place = [.. repository.FetchSavedPlaces(broId)];

        return place.Select(p => new PlaceBookmark(
            PlaceId: p.Id,
            Name: p.Name,
            Description: p.Description,
            Picture: p.Picture?.Small));
    }

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
