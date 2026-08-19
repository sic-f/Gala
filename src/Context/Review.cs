using Core.Dto;
using Core.Exceptions;
using Core.Interfaces;

namespace Context;

public class Review(IReviewRepository repository, ISavedRepository savedRepository)
{
    public async Task<IEnumerable<ReviewDto>> FetchPaginatedReviews(int offset, int limit, Guid placeId, string? broId)
    {
        IEnumerable<ReviewDto> reviewDtos = GetSavedDtos(repository.FetchPaginated(offset, limit, placeId));

        foreach (ReviewDto dto in reviewDtos)
        {
            dto.Reviewer = await GetReviewerNameAsync(dto.ReviewerId);
        }

        if (broId is not null)
        {
            foreach (ReviewDto dto in reviewDtos)
            {
                dto.BookmarkId = await GetSavedIdAsync(dto.Id, Guid.Parse(broId));
            }
        }

        return reviewDtos;
    }

    public ReviewDto GetReview(Guid id)
    {
        return repository.NoTrackingFetchWithReviewer(id);
    }

    public async Task<Core.PlaceAggregate.Review> WriteAsync(ReviewDetails details, Guid broId)
    {
        var review = new Core.PlaceAggregate.Review();

        review.SetDetails(details, broId);

        await repository.CreateReviewAsync(review);

        return review;
    }

    public async Task ModifyAsync(Guid id, ReviewDetails details, Guid broId)
    {
        Core.PlaceAggregate.Review review = await repository.FetchAsync(id);

        if (review.PlaceId != details.PlaceId && review.ReviewerId != broId)
        {
            throw new UnauthorizedReviewModificationException(
                "❌👦🏣 You did not write this review, you are not allowed to modify it.");
        }

        review.UpdateDetails(details);

        await repository.ModifyAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        Core.PlaceAggregate.Review review = await repository.NoTrackingFetchAsync(id);

        await repository.DeleteAsync(review);
    }

    private static IEnumerable<ReviewDto> GetSavedDtos(IQueryable<Core.PlaceAggregate.Review> reviews)
    {
        return [.. reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            Headline = r.Headline,
            Content = r.Content,
            PlaceId = r.PlaceId,
            ReviewerId = r.ReviewerId,
            CreatedAt = r.CreatedAt,
            LastModified = r.LastModified,
        })];
    }

    private async Task<Guid?> GetSavedIdAsync(Guid saveableId, Guid broId)
    {
        return await savedRepository.GetSavedIdAsync(saveableId, broId);
    }

    private async Task<string> GetReviewerNameAsync(Guid reviewerId)
    {
        return await repository.GetReviewerNameAsync(reviewerId);
    }
}
