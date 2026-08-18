using Core.Dto;
using Core.PlaceAggregate;
using Infrastructure.Identity;
using IntegrationTests.Errors;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test;

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

            _place = await PlaceFactory.CreateAsync(app.BroId, app.Services);
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

    [Collection("Application Factory Collection")]
    public class UpdateTest(ReviewScope app) : BaseTest(app), IClassFixture<ReviewScope>
    {
        private readonly ReviewErrors _errors = new(app);
        private Review _review = default!;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Place place = await PlaceFactory.CreateAsync(app.BroId, app.Services);
            Review newReview = new() { Headline = "Headline", Content = "Content", PlaceId = place.Id, ReviewerId = app.BroId };
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

    [Collection("Application Factory Collection")]
    public class DeleteTest(ReviewScope app) : BaseTest(app), IClassFixture<ReviewScope>
    {
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task DeleteReviewTestAsync()
        {
            Bro bro = await BroFactory.CreateAsync(app.Services);
            Place place = await PlaceFactory.CreateAsync(bro.Id, app.Services);
            Review newReview = new() { ReviewerId = bro.Id, PlaceId = place.Id };
            Review review = await ReviewFactory.CreateAsync(newReview, app.Services);

            await app.DeleteReviewAsync(review.Id);

            Assert.Equal(HttpStatusCode.NoContent, app.ResponseStatusCode);
        }
    }

    [Collection("Application Factory Collection")]
    public class FetchTest(ReviewScope app) : BaseTest(app), IClassFixture<ReviewScope>
    {
        const int Offset = 5;
        const int NumberOfReviewsToFetch = 5;
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task PaginatedFetchTestAsync()
        {
            Place place = await PlaceFactory.CreateAsync(app.BroId, app.Services);
            Place otherPlace = await PlaceFactory.CreateAsync(app.BroId, app.Services);
            IEnumerable<Review> newReviews = await BuildReviews(place.Id, otherPlace.Id);
            IEnumerable<Review> reviews = await ReviewFactory.CreateAsync(newReviews, app.Services);

            await app.FetchPaginatedReviewsAsync(offset: Offset, limit: NumberOfReviewsToFetch, place.Id);

            IEnumerable<ReviewDto> content = await app.FetchPaginatedReviewsSuccessResponseAsync();

            Assert.Collection<ReviewDto>(content,
                r => Assert.Equal("4th Review", r.Content),
                r => Assert.Equal("2nd Review", r.Content),
                r => Assert.Equal("1st Review", r.Content));
        }

        private async Task<IEnumerable<Review>> BuildReviews(Guid placeId, Guid otherPlaceId)
        {
            return
            [
                new() { Content = "1st Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1) },
                new() { Content = "2nd Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3) },
                new() { Content = "3rd Review", PlaceId = otherPlaceId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(5) },
                new() { Content = "4th Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(7) },
                new() { Content = "5th Review", PlaceId = otherPlaceId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(9) },
                new() { Content = "6th Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(11) },
                new() { Content = "7th Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(13) },
                new() { Content = "8th Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(15) },
                new() { Content = "9th Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(17) },
                new() { Content = "10th Review", PlaceId = placeId, ReviewerId = app.BroId, CreatedAt = DateTimeOffset.UtcNow.AddSeconds(19) },
            ];
        }
    }
}
