using Core.PlaceAggregate;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Dto;
using Utility.Factories;

namespace IntegrationTests.Test.Saved;

[Collection("Application Factory Collection")]
public class SavePlace(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task SavePlaceTestAsync()
    {
        PlaceAttributes attributes = new() { Name = "Name", Address = "Address", BroId = app.Bro.Id };
        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app.Services);

        await app.SavePlaceAsync(place.Id);

        var responseBody = await app.SavePlaceSuccessResponseAsync();

        Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
        Assert.Equal(place.Id, responseBody.SaveableId);
        Assert.Equal(app.Bro.Id, responseBody.SaverId);
    }
}
