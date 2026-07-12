using Core.PlaceAggregate;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Dto;
using Utility.Factories;

namespace IntegrationTests.Test.Saved;

public class SavePlace(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task SavePlaceTestAsync()
    {
        PlaceAttributes attributes = new() { Name = "Name", Address = "Address", BroId = app.Bro.Id };
        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app.Services);

        await app.SavePlaceAsync(place.Id);

        Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
    }
}
