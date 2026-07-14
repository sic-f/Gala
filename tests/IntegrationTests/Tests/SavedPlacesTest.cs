using Core.Dto;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;

namespace IntegrationTests.Tests;

public class SavedPlacesTest()
{
    public class SaveTest
    {
        [Collection("Application Factory Collection")]
        public class SavePlaceTest(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task SavePlaceTestAsync()
            {
                Place newPlace = new() { Name = "Name", Address = "Address", BroId = app.Bro.Id };
                Place place = await PlaceFactory.CreateAsync(newPlace, app.Services);

                await app.SavePlaceAsync(place.Id);

                var responseBody = await app.SavePlaceSuccessResponseAsync();

                Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
                Assert.Equal(place.Id, responseBody.SaveableId);
                Assert.Equal(app.Bro.Id, responseBody.SaverId);
            }
        }
    }

    public class FetchTest
    {
        [Collection("Application Factory Collection")]
        public class FetchSavedPlacesTest(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
        {
            private IEnumerable<SavedPlaceResponse> _response = default!;
            private Bro _bro = default!;

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();
                _bro = await BroFactory.CreateAsync(app.Services);
                IEnumerable<SavedPlace> builtSavedPlaces =
                [
                    new SavedPlace() { SaverId = _bro.Id },
                    new SavedPlace() { SaverId = _bro.Id },
                    new SavedPlace() { SaverId = _bro.Id },
                ];

                await SavedPlaceFactory.CreateAsync(builtSavedPlaces, app.Services);

                _response = await app.GetAllSavedPlacesAsync(_bro.Id);
            }
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task FetchTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.Equal(_bro.Id, sp.SaverId),
                    sp => Assert.Equal(_bro.Id, sp.SaverId),
                    sp => Assert.Equal(_bro.Id, sp.SaverId));
            }

            [Fact]
            public async Task FetchWithIncorrectBroIdTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.NotEqual(app.Bro.Id, sp.SaverId),
                    sp => Assert.NotEqual(app.Bro.Id, sp.SaverId),
                    sp => Assert.NotEqual(app.Bro.Id, sp.SaverId));
            }
        }
    }
}
