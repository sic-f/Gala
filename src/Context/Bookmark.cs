using Core.Dto;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Context;

public class Bookmark(ISavedPlaceRepository repository)
{
    public IEnumerable<PlaceBookmark> FetchBookmarks(DateTimeOffset offset, int limit, Guid broId)
    {
        var (places, savedPlaces) = repository.FetchSavedPlaces(offset, limit, broId);

        return BuildPlaceBookmarks(places, savedPlaces);
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

    private static IEnumerable<PlaceBookmark> BuildPlaceBookmarks(IEnumerable<Place> places, IList<SavedPlace> savedPlaces)
    {
        return savedPlaces.Join(
            places,
            savedPlace => savedPlace.SaveableId,
            place => place.Id,
            (savedPlace, place) => new PlaceBookmark(
                PlaceId: place.Id,
                Name: place.Name,
                Description: place.Description,
                Picture: place.Picture?.Small,
                CreatedAt: savedPlace.CreatedAt));
    }
}
