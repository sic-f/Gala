using Core.PlaceAggregate;
using Infrastructure.Data;
using IntegrationTests.Scopes;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;
using Utility.Factories;

namespace IntegrationTests.Places.Delete;

[Collection("Application Factory Collection")]
public class Delete(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task DeletePlaceTestAsync()
    {
        Place place = await GivenPlaceAsync();
        await WhenBroDeletesPlace(place.Id);
        ThenPlaceIsDeleted(place.Id);
    }

    private async Task<Place> GivenPlaceAsync()
    {
        PlaceAttributes attributes = new() { Name = "Test Name", Address = "Test Address", BroId = app.Bro.Id };

        return await PlaceFactory.CreatePlaceAsync(attributes, app.Services);
    }

    private async Task WhenBroDeletesPlace(Guid placeId)
    {
        await app.DeletePlaceAsync(placeId);
    }

    private void ThenPlaceIsDeleted(Guid placeId)
    {
        var exception = Record.Exception(() => app.Response.EnsureSuccessStatusCode());
        Assert.Null(exception);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        Assert.Throws<InvalidOperationException>(() => context.Place.First(p => p.Id == placeId));
    }

    [Fact]
    public async Task DeleteNonExistentPlaceTestAsync()
    {
        var nonExistentPlaceId = Guid.NewGuid();

        Place place = await GivenPlaceAsync();
        await WhenBroDeletesNonExistentPlace(placeId: nonExistentPlaceId);
        ThenPlaceIsNotDeleted(place.Id);
    }

    private async Task WhenBroDeletesNonExistentPlace(Guid placeId)
    {
        await app.DeletePlaceAsync(placeId);
    }

    private void ThenPlaceIsNotDeleted(Guid id)
    {
        var exception = Record.Exception(() => app.Response.EnsureSuccessStatusCode());
        Assert.NotNull(exception);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        var dbException = Record.Exception(() => context.Place.First(p => p.Id == id));
        Assert.Null(dbException);
    }
}