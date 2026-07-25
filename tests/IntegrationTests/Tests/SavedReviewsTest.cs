using Core.PlaceAggregate;
using Core.SavedAggregate;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Tests;

public class SavedReviewsTest
{
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
}
