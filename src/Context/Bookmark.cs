using Core.Dto;
using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Context;

public class Bookmark(ISavedRepository repository, IPlaceRepository placeRepository)
{
    public IEnumerable<PlaceBookmark> FetchBookmarks(DateTimeOffset offset, int limit, Guid broId)
    {
        var (places, savedPlaces) = repository.FetchSavedPlaces(offset, limit, broId);

        return BuildPlaceBookmarks(places, savedPlaces);
    }

    public async Task<SavedPlace> SavePlaceAsync(Guid placeId, Guid broId)
    {
        Place place = await placeRepository.FetchPlaceAsync(placeId);

        if (place.BroId == broId)
        {
            throw new SaveOwnPlaceException("You can not save your own contributed place.");
        }

        SavedPlace savedPlace = new()
        {
            SaveableId = placeId,
            SaverId = broId,
        };

        return await repository.CreateSavedPlaceAsync(savedPlace);
    }

    public async Task<SavedReview> SaveReviewAsync(Guid reviewId, Guid broId)
    {
        SavedReview savedReview = new()
        {
            SaveableId = reviewId,
            SaverId = broId,
        };

        return await repository.CreateSavedReviewAsync(savedReview);
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
