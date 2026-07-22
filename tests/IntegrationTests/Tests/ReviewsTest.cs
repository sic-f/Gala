using Core.Dto;
using Core.PlaceAggregate;
using IntegrationTests.Errors;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;
using Utility.Helpers;

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
            Assert.Equal("Must write a review or cancel.", _errors.Content);
        }

        [Fact]
        public async Task WithContentTestAsync()
        {
            var review = new Review { Content = "Content", PlaceId = _place.Id };

            await app.SubmitReviewAsync(review);

            Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
        }
    }

    public class UpdateTest(ReviewScope app) : BaseTest(app), IClassFixture<ReviewScope>
    {
        private readonly ReviewErrors _errors = new(app);
        private Review _review = default!;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Place place = await PlaceFactory.CreateAsync(app.Bro.Id, app.Services);
            Review newReview = new() { Headline = "Headline", Content = "Content", PlaceId = place.Id, ReviewerId = app.Bro.Id };
            _review = await ReviewFactory.CreateAsync(newReview, app.Services);
        }

        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task WithBlankContentTestAsync()
        {
            ReviewDetails details = new() { Content = string.Empty };

            await app.UpdateReviewAsync(_review.Id, details);

            Assert.Equal("Must write a review or cancel.", _errors.Content);
        }

        [Fact]
        public async Task WithNewHeadlineAndContentTestAsync()
        {
            ReviewDetails details = new() { Headline = "New Headline", Content = "New Content" };

            await app.UpdateReviewAsync(_review.Id, details);

            Review updatedReview = RepositoryHelper.GetFirstRecord<Review>(app.Services);

            Assert.Equal("New Headline", updatedReview.Headline);
            Assert.Equal("New Content", updatedReview.Content);
        }
    }
}
