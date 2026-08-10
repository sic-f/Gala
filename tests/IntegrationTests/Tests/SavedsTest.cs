using Core.Dto;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using System.Net;
using System.Net.Http.Json;
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
        SavedPlace savedPlace = new() { SaveableId = place.Id, SaverId = app.Bro.Id };
        SavedReview savedReview = new() { SaveableId = newReview.Id, SaverId = app.Bro.Id };
        await ReviewFactory.CreateAsync(newReview, app.Services);
        await SavedPlaceFactory.CreateAsync(savedPlace, app.Services);
        await SavedReviewFactory.CreateAsync(savedReview, app.Services);

        await app.GetAllSavedPlacesAsync(
            GetFormattedDateTimeOffset(DateTimeOffset.UtcNow.AddSeconds(1)),
            FetchLimit);

        var response = (await app.Response.Content.ReadFromJsonAsync<IEnumerable<PlaceBookmark>>());


        Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
    }

    private static string GetFormattedDateTimeOffset(DateTimeOffset dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.sssssssZ");
    }
}
