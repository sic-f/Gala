using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Utility.Dto;
using Utility.Factories;
using Utility.Scopes;

namespace IntegrationTests.Places.Delete;

[Collection("Application Factory Collection")]
public class PublicAccess(PlaceScope app) : BaseTest(app, isAuthenticated: false), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task UnauthorizedDeleteTestAsync()
    {
        Place place = await GivenPlaceAsync();
        await WhenBroDeletesPlaceUnauthenticatedOrUnregisteredAsync(place.Id);
        ThenBroWillNotBeAbleToDeletePlace(place.Id);
    }

    private async Task<Place> GivenPlaceAsync()
    {
        Bro bro = await BroFactory.CreateBroAsync("test@example.net", "Hey@123", app);
        PlaceAttributes attributes = new() { Name = "Test Name", Address = "Test Adress", BroId = bro.Id };

        return await PlaceFactory.CreatePlaceAsync(attributes, app);
    }

    private async Task WhenBroDeletesPlaceUnauthenticatedOrUnregisteredAsync(Guid placeId)
    {
        await app.DeletePlaceAsync(placeId);
    }

    private void ThenBroWillNotBeAbleToDeletePlace(Guid placeId)
    {
        var exception = Record.Exception(() => app.Response.EnsureSuccessStatusCode());
        Assert.NotNull(exception);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        Assert.Equal(HttpStatusCode.Unauthorized, app.ResponseStatusCode);
        var dbException = Record.Exception(() => context.Place.First(p => p.Id == placeId));
        Assert.Null(dbException);
    }
}
