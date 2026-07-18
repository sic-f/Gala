using Core.PlaceAggregate;
using IntegrationTests.Errors;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;

namespace IntegrationTests.Tests;

public class ReviewsTest
{
    [Collection("Application Factory Collection")]
    public class SubmitReviewTest(ReviewScope app) : BaseTest(app), IClassFixture<ReviewScope>
    {
        private readonly ReviewErrors _errors = new(app);
        private Place _place = default!;

        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _place = await PlaceFactory.CreateAsync(app.Bro.Id, app.Services);
        }

        [Fact]
        public async Task WithBlankContentTestAsync()
        {
            var review = new Review { Content = string.Empty, PlaceId = _place.Id };

            await app.SubmitReviewAsync(review);

            Assert.Equal(HttpStatusCode.BadRequest, app.ResponseStatusCode);
            Assert.Equal("The Content field is required.", _errors.Content);
        }

        [Fact]
        public async Task WithContentTestAsync()
        {
            var review = new Review { Content = "Content", PlaceId = _place.Id };

            await app.SubmitReviewAsync(review);

            Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
        }
    }
}
