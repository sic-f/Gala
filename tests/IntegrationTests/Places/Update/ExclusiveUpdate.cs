using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;
using Utility.Factories;
using Utility.Scopes;

namespace IntegrationTests.Places.Update;

[Collection("Application Factory Collection")]
public class ExclusiveUpdate(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task ExclusivePlaceUpdateTestAsync()
    {
        (PlaceAttributes attributes, Place place) = await GivenPlaceOwnedByAnotherBroAsync();
        await WhenBroTriesToUpdatePlaceOwnedByAnotherBroAsync(place.Id);
        ThenBroWillNotBeAbleToUpdateThePlace(place.Id);
    }

    private async Task<(PlaceAttributes attributes, Place place)> GivenPlaceOwnedByAnotherBroAsync()
    {
        Bro placeOwner = await BroFactory.CreateBroAsync("test@example.org", "Hey@123", app);
        PlaceAttributes attributes = new(Name: "Test Name", Address: "Test Address", BroId: placeOwner.Id);
        Place place = PlaceFactory.CreatePlace(attributes, app);

        return (attributes, place);
    }

    private async Task WhenBroTriesToUpdatePlaceOwnedByAnotherBroAsync(Guid placeId)
    {
        PlaceUpdateAttributes updateAttributes = new(Name: "Test Name Update", Address: "Test Address Update");
        await app.UpdatePlaceAsync(placeId, updateAttributes);
    }

    private void ThenBroWillNotBeAbleToUpdateThePlace(Guid placeId)
    {
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
