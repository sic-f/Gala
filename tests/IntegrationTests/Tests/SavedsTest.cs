using Core.Dto;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Tests;

[Collection("Application Factory Collection")]
public class FetchSavedsTest(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
{
    private const int FetchLimit = 5;
    private Bro _bro = default!;
    private Place _place = default!;
    private Review _review = default!;
    private SavedPlace _savedPlace = default!;
    private SavedReview _savedReview = default!;
    private IEnumerable<SavedDto> _savedDtos = [];

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _bro = await BroFactory.CreateAsync(app.Services);
        _place = await PlaceFactory.CreateAsync(_bro.Id, app.Services);
        _review = new()
        {
            Content = "Fantastic experience!",
            PlaceId = _place.Id,
            ReviewerId = _bro.Id
        };
        var excludedReview = new Review
        {
            Content = "Excluded review",
            PlaceId = _place.Id,
            ReviewerId = app.Bro.Id
        };
        await ReviewFactory.CreateAsync(_review, app.Services);
        await ReviewFactory.CreateAsync(excludedReview, app.Services);
        _savedPlace = new() { SaveableId = _place.Id, SaverId = app.Bro.Id };
        _savedReview = new()
        {
            SaveableId = _review.Id,
            SaverId = app.Bro.Id,
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1)
        };
        var excludedSavedReview = new SavedReview
        {
            SaveableId = excludedReview.Id,
            SaverId = _bro.Id,
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(2)
        };
        await SavedPlaceFactory.CreateAsync(_savedPlace, app.Services);
        await SavedReviewFactory.CreateAsync(_savedReview, app.Services);
        await SavedReviewFactory.CreateAsync(excludedSavedReview, app.Services);

        _savedDtos = await app.FetchSavedsAsync(
            DateHelper.GetFormattedDateTimeOffset(DateTimeOffset.UtcNow.AddSeconds(2)),
            FetchLimit);
    }

    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task IdsTestAsync()
    {
        Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
        Assert.Collection(_savedDtos,
            s => Assert.Equal(_review.Id, s.SaveableId),
            s => Assert.Equal(_place.Id, s.SaveableId));
    }

    [Fact]
    public async Task HeadlineTestAsync()
    {
        Assert.Collection(_savedDtos,
            savedDto => Assert.Equal(savedDto.Headline, _review.Content),
            savedDto => Assert.Equal(savedDto.Headline, _place.Name));
    }

    [Fact]
    public async Task PictureTestAsync()
    {
        Assert.Collection(_savedDtos,
            savedDto => Assert.Null(savedDto.Picture),
            savedDto => Assert.Null(savedDto.Picture));
    }

    [Fact]
    public async Task CreatedAtTestAsync()
    {
        Assert.Collection(_savedDtos,
            savedDto => Assert.Equal(savedDto.CreatedAt, _savedReview.CreatedAt),
            savedDto => Assert.Equal(savedDto.CreatedAt, _savedPlace.CreatedAt));
    }
}
