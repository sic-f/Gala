using Core.Dto;
using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Context;

public class Bookmark(ISavedRepository repository, IPlaceRepository placeRepository)
{
    public IOrderedEnumerable<SavedDto> FetchBookmarks(DateTimeOffset offset, int limit, Guid broId)
    {
        return repository.FetchSaveds(offset, limit, broId);
    }

    public IEnumerable<SavedDto> GetSavedPlaces(DateTimeOffset offset, int limit, Guid broId)
    {
        return repository.GetSavedPlaces(offset, limit, broId);
    }

    public IEnumerable<SavedDto> GetSavedReviews(DateTimeOffset offset, int limit, Guid broId)
    {
        return repository.GetSavedReviews(offset, limit, broId);
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

    public async Task DeleteSavedAsync(Guid savedId)
    {
        Core.SavedAggregate.Saved saved = await repository.FetchSavedAsync(savedId);

        await repository.DeleteSavedAsync(saved);
    }
}
