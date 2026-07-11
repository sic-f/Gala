using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using Ui.Dto;
using Utility.Dto;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test.Places.Update;

[Collection("Application Factory Collection")]
public class ExclusiveUpdate(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task ExclusivePlaceUpdateTestAsync()
    {
        (PlaceAttributes _, Place place) = await GivenPlaceOwnedByAnotherBroAsync();
        await WhenBroTriesToUpdatePlaceOwnedByAnotherBroAsync(place.Id);
        await ThenBroWillNotBeAbleToUpdateThePlace(place.Id);
    }

    private async Task<(PlaceAttributes attributes, Place place)> GivenPlaceOwnedByAnotherBroAsync()
    {
        Bro placeOwner = await BroFactory.CreateBroAsync("test@example.org", "Hey@123", app.Services);
        PlaceAttributes attributes = new()
        {
            Name = "Test Name",
            Address = "Test Address",
            BroId = placeOwner.Id
        };
        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app.Services);

        return (attributes, place);
    }

    private async Task WhenBroTriesToUpdatePlaceOwnedByAnotherBroAsync(Guid placeId)
    {
        PlaceDetails updateAttributes = new()
        {
            Name = "Test Name Update",
            Address = "Test Address Update",
            Picture = ImageHelper.MockImageFormFile()
        };

        await app.UpdatePlaceAsync(placeId, updateAttributes);
    }

    private async Task ThenBroWillNotBeAbleToUpdateThePlace(Guid placeId)
    {
        Assert.Equal(
            "❌👦🏣 Bro not allowed to update other people's places.",
            (await app.Response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("detail").ToString());
        var exception = Record.Exception(() => app.Response.EnsureSuccessStatusCode());
        Assert.NotNull(exception);

        var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

        Place place = db.Place.First(p => p.Id == placeId);

        Assert.NotEqual("Test Name Update", place.Name);
        Assert.NotEqual("Test Address Update", place.Address);
    }
}
