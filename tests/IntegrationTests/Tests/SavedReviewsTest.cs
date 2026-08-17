using Core.PlaceAggregate;
using Core.SavedAggregate;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Tests;

public class SavedReviewsTest
{
    [Collection("Application Factory Collection")]
    public class SaveReviewsTest(SavedReviewScope app) : BaseTest(app), IClassFixture<SavedReviewScope>
    {
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task SaveReviewTestAsync()
        {
            Place place = await PlaceFactory.CreateAsync(app.Bro.Id, app.Services);
            Review newReview = new() { Content = "Content", PlaceId = place.Id, ReviewerId = app.Bro.Id };
            Review review = await ReviewFactory.CreateAsync(newReview, app.Services);

            await app.SavedReviewAsync(review.Id);

            SavedReview savedReview = RepositoryHelper.GetFirstRecord<SavedReview>(app.Services);

            Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
            Assert.Equal(app.Bro.Id, savedReview.SaverId);
            Assert.Equal(review.Id, savedReview.SaveableId);
        }
    }

    [Collection("Application Factory Collection")]
    public class FetchReviewsTest(SavedReviewScope app) : BaseTest(app), IClassFixture<SavedReviewScope>
    {
        IList<SavedReview> _savedReviews = [];

        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Place place = await PlaceFactory.CreateAsync(app.Bro.Id, app.Services);


            IEnumerable<SavedReview> buildReviews = BuildReviews(saveableId: place.Id, saverId: app.Bro.Id);
            _savedReviews = [.. await SavedReviewFactory.CreateAsync(buildReviews, app.Services)];
        }

        [Fact]
        public async Task IdsTestAsync()
        {
            Assert.Collection(_savedReviews,
                review => Assert.Equal(expected: _savedReviews[5].Id, actual: review.Id),
                review => Assert.Equal(expected: _savedReviews[4].Id, actual: review.Id),
                review => Assert.Equal(expected: _savedReviews[3].Id, actual: review.Id),
                review => Assert.Equal(expected: _savedReviews[2].Id, actual: review.Id),
                review => Assert.Equal(expected: _savedReviews[1].Id, actual: review.Id));
        }

        private static IList<SavedReview> BuildReviews(Guid saveableId, Guid saverId)
        {
            return [
                new SavedReview
                {
                    SaveableId = saveableId,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1)
                },
                new SavedReview
                {
                    SaveableId = saveableId,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3)
                },
                new SavedReview
                {
                    SaveableId = saveableId,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(5)
                },
                new SavedReview
                {
                    SaveableId = saveableId,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(7)
                },
                new SavedReview
                {
                    SaveableId = saveableId,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(9)
                },
                new SavedReview
                {
                    SaveableId = saveableId,
                    SaverId = saverId,
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(11)
                }
            ];
        }

    }
}
