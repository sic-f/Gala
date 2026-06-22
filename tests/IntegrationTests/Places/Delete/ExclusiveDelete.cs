using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;
using Utility.Factories;
using Utility.Scopes;

namespace IntegrationTests.Places.Delete;

[Collection("Application Factory Collection")]
public class ExclusiveDelete(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task PlaceOwnerExclusiveDeleteTestAsync()
    {
        Place place = await GivenPlaceAndBroNotThePlaceOwnerAsync();
        await WhenPlaceIsDeletedByNotTheOwnerAsync(place.Id);
        ThenPlaceWillNotBeDeleted(place.Id);
    }

    private async Task<Place> GivenPlaceAndBroNotThePlaceOwnerAsync()
    {
        Bro placeOwner = await BroFactory.CreateBroAsync("test@example.org", "Hey@123", app);
        PlaceAttributes attributes = new(Name: "Test Name", Address: "Test Address", BroId: placeOwner.Id);
        return PlaceFactory.CreatePlace(attributes, app);
    }

    private async Task WhenPlaceIsDeletedByNotTheOwnerAsync(Guid placeId)
    {
        await app.DeletePlaceAsync(placeId);
    }

    private void ThenPlaceWillNotBeDeleted(Guid placeId)
    {
        var exception = Record.Exception(() => app.Response.EnsureSuccessStatusCode());
        Assert.NotNull(exception);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        var dbException = Record.Exception(() => context.Place.First(p => p.Id == placeId));
        Assert.Null(dbException);
    }
}
