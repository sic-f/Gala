using Core.Dto;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;

namespace Context;

public class Bookmark(ISavedRepository repository, IPlaceRepository placeRepository)
{
    public async Task<IEnumerable<SavedDto>> FetchBookmarksAsync(DateTimeOffset offset, int limit, Guid broId)
    {
        var (places, savedPlaces, reviews, savedReviews) = repository.FetchSavedPlaces(offset, limit, broId);

        return await BuildPlaceBookmarksAsync(places, savedPlaces, reviews, savedReviews);
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

    private async Task<IEnumerable<SavedDto>> BuildPlaceBookmarksAsync(
        IEnumerable<Place> places,
        IEnumerable<Core.SavedAggregate.Saved> savedPlaces,
        IEnumerable<Core.PlaceAggregate.Review> reviews,
        IEnumerable<Core.SavedAggregate.Saved> savedReviews)
    {
        List<SavedDto> savedPlaceDtos = [.. savedPlaces.Join(
            places,
            savedPlace => savedPlace.SaveableId,
            place => place.Id,
            (savedPlace, place) => new SavedDto
            {
                SaveableId = place.Id,
                Headline = place.Name,
                Picture = place.Picture?.Small,
                CreatedAt = savedPlace.CreatedAt
            })];

        List<SavedDto> savedReviewDtos = [ ..savedReviews.Join(
            reviews,
            savedReview => savedReview.SaveableId,
            review => review.Id,
            (savedReview, review) => new SavedDto
            {
                SaveableId = review.Id,
                Headline = review.Content,
                CreatedAt = savedReview.CreatedAt
            })];

        foreach (SavedDto savedReviewDto in savedReviewDtos)
        {
            savedReviewDto.Picture = await GetReviewerAvatarAsync(savedReviewDto.SaveableId);
        }

        savedPlaceDtos.AddRange(savedReviewDtos);

        return savedPlaceDtos.OrderByDescending(dto => dto.CreatedAt);
    }

    private async Task<string?> GetReviewerAvatarAsync(Guid reviewId)
    {
        AccountAvatar? avatar = await repository.GetReviewerAvatarAsync(reviewId);

        return avatar?.Normal;
    }
}
