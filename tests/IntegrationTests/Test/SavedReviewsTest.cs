using Core.Dto;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test;

public class SavedReviewsTest
{
    [Collection("Application Factory Collection")]
    public class SaveReviewsTest(SavedReviewScope app) : BaseTest(app), IClassFixture<SavedReviewScope>
    {
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task SaveReviewTestAsync()
        {
            Place place = await PlaceFactory.CreateAsync(app.BroId, app.Services);
            Review newReview = new() { Content = "Content", PlaceId = place.Id, ReviewerId = app.BroId };
            Review review = await ReviewFactory.CreateAsync(newReview, app.Services);

            await app.SavedReviewAsync(review.Id);

            SavedReview savedReview = RepositoryHelper.GetFirstRecord<SavedReview>(app.Services);

            Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
            Assert.Equal(app.BroId, savedReview.SaverId);
            Assert.Equal(review.Id, savedReview.SaveableId);
        }
    }

    [Collection("Application Factory Collection")]
    public class FetchReviewsTest(SavedReviewScope app) : BaseTest(app), IClassFixture<SavedReviewScope>
    {
        private const int FetchLimit = 5;
        IList<Review> _reviews = [];
        IList<SavedReview> _savedReviews = [];
        IEnumerable<SavedDto> _savedDtos = [];

        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Place place = await PlaceFactory.CreateAsync(app.BroId, app.Services);
            _reviews = [.. await ReviewFactory.CreateAsync(
                BuildReviews(placeId: place.Id, reviewerId: app.BroId),
                app.Services)];
            IEnumerable<SavedReview> buildSavedReviews = BuildSavedReviews(saveableId: place.Id, saverId: app.BroId);
            _savedReviews = [.. await SavedReviewFactory.CreateAsync(buildSavedReviews, app.Services)];

            _savedDtos = (await app.GetSavedReviewsAsync(DateHelper.GetFormattedDateTimeOffset(
                DateTimeOffset.UtcNow.AddSeconds(13)),
                FetchLimit))!;
        }

        [Fact]
        public async Task IdsTestAsync()
        {
            Assert.Collection(_savedDtos,
                savedDto => Assert.Equal(expected: _reviews[5].Id, actual: savedDto.SaveableId),
                savedDto => Assert.Equal(expected: _reviews[4].Id, actual: savedDto.SaveableId),
                savedDto => Assert.Equal(expected: _reviews[3].Id, actual: savedDto.SaveableId),
                savedDto => Assert.Equal(expected: _reviews[2].Id, actual: savedDto.SaveableId),
                savedDto => Assert.Equal(expected: _reviews[1].Id, actual: savedDto.SaveableId));
        }

        private IList<SavedReview> BuildSavedReviews(Guid saveableId, Guid saverId)
        {
            return [
                new SavedReview
                {
                    SaveableId = _reviews[0].Id,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1)
                },
                new SavedReview
                {
                    SaveableId = _reviews[1].Id,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3)
                },
                new SavedReview
                {
                    SaveableId = _reviews[2].Id,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(5)
                },
                new SavedReview
                {
                    SaveableId = _reviews[3].Id,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(7)
                },
                new SavedReview
                {
                    SaveableId = _reviews[4].Id,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(9)
                },
                new SavedReview
                {
                    SaveableId = _reviews[5].Id,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(11)
                }
            ];
        }

        private static IList<Review> BuildReviews(Guid placeId, Guid reviewerId)
        {
            return [
                new Review
                {
                    PlaceId = placeId,
                    ReviewerId = reviewerId,
                    Content = "Review A",
                },
                new Review
                {
                    PlaceId = placeId,
                    ReviewerId = reviewerId,
                    Content = "Review B",
                },
                new Review
                {
                    PlaceId = placeId,
                    ReviewerId = reviewerId,
                    Content = "Review C",
                },
                new Review
                {
                    PlaceId = placeId,
                    ReviewerId = reviewerId,
                    Content = "Review D",
                },
                new Review
                {
                    PlaceId = placeId,
                    ReviewerId = reviewerId,
                    Content = "Review E",
                },
                new Review
                {
                    PlaceId = placeId,
                    ReviewerId = reviewerId,
                    Content = "Review F",
                },
            ];
        }
    }
}
