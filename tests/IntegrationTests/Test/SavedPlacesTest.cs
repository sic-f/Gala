using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Dto;
using Utility.Factories;

namespace IntegrationTests.Test;

public class SavedPlacesTest()
{
    [Collection("Application Factory Collection")]
    public class SavePlaceTest(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
    {
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task SavePlaceTestAsync()
        {
            PlaceAttributes attributes = new() { Name = "Name", Address = "Address", BroId = app.Bro.Id };
            Place place = await PlaceFactory.CreateAsync(attributes, app.Services);

            await app.SavePlaceAsync(place.Id);

            var responseBody = await app.SavePlaceSuccessResponseAsync();

            Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
            Assert.Equal(place.Id, responseBody.SaveableId);
            Assert.Equal(app.Bro.Id, responseBody.SaverId);
        }
    }

    [Collection("Application Factory Collection")]
    public class FetchSavedPlacesTest(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
    {
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task FetchTestAsync()
        {
            Bro bro = await BroFactory.CreateAsync(app.Services);
            IEnumerable<SavedPlace> builtSavedPlaces =
            [
                new SavedPlace() { SaverId = bro.Id },
                new SavedPlace() { SaverId = bro.Id },
                new SavedPlace() { SaverId = bro.Id },
            ];

            await SavedPlaceFactory.CreateAsync(builtSavedPlaces, app.Services);

            var response = await app.GetAllSavedPlacesAsync();

            Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
            Assert.Collection(response,
                sp => Assert.NotEqual(app.Bro.Id, sp.SaverId),
                sp => Assert.NotEqual(app.Bro.Id, sp.SaverId),
                sp => Assert.NotEqual(app.Bro.Id, sp.SaverId));
        }
    }
}
