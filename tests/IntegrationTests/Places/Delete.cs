using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;
using Utility.Factories;
using Utility.Scopes;

namespace IntegrationTests.Places;

public class Delete(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task DeletePlaceTestAsync()
    {
        Place place = GivenPlace();
        await WhenBroDeletesPlace(place.Id);
        await ThenPlaceIsDeletedAsync(place.Id);
    }

    private Place GivenPlace()
    {
        PlaceAttributes attributes = new(Name: "Test Name", Address: "Test Address", BroId: app.Bro.Id);

        return PlaceFactory.CreatePlace(attributes, app);
    }

    private async Task WhenBroDeletesPlace(Guid placeId)
    {
        await app.DeletePlaceAsync(placeId);
    }

    private async Task ThenPlaceIsDeletedAsync(Guid id)
    {
        var exception = await Record.ExceptionAsync(async () => app.Response.EnsureSuccessStatusCode());
        Assert.Null(exception);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        Assert.Throws<InvalidOperationException>(() => context.Place.First(p => p.Id == id));
    }
}