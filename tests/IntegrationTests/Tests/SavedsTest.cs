using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;

namespace IntegrationTests.Tests;

[Collection("Application Factory Collection")]
public class SavedsTest(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
{
    private const int FetchLimit = 5;

    [Fact]
    public async Task FetchSavedPlacesAndReviewsTestAsync()
    {
        Bro bro = await BroFactory.CreateAsync(app.Services);
        Place place = await PlaceFactory.CreateAsync(bro.Id, app.Services);
        Review newReview = new()
        {
            Headline = string.Empty,
            Content = string.Empty,
            PlaceId = place.Id,
            ReviewerId = bro.Id
        };
        await ReviewFactory.CreateAsync(newReview, app.Services);
        SavedPlace newSavedPlace = new() { SaveableId = place.Id, SaverId = app.Bro.Id };
        SavedReview newSavedReview = new() { SaveableId = newReview.Id, SaverId = app.Bro.Id };
        SavedPlace savedPlace = await SavedPlaceFactory.CreateAsync(newSavedPlace, app.Services);
        SavedReview savedReview = await SavedReviewFactory.CreateAsync(newSavedReview, app.Services);

        var savedDtos = await app.FetchSavedsAsync(
            GetFormattedDateTimeOffset(DateTimeOffset.UtcNow.AddSeconds(1)),
            FetchLimit);

        Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
        Assert.Collection(savedDtos!,
            s => Assert.Equal(s.SaveableId, savedReview.Id),
            s => Assert.Equal(s.SaveableId, savedPlace.Id));
    }
    // TODO: Extract to one place.
    private static string GetFormattedDateTimeOffset(DateTimeOffset dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.sssssssZ");
    }
}
