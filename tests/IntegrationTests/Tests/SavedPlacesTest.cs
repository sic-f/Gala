using Core.Dto;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using Microsoft.Extensions.DependencyInjection;
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
            private Place[] _places = default!;

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();

                _bro = await BroFactory.CreateAsync(app.Services);
                _places = [.. await PlaceFactory.CreateAsync(BuildPlaces(app.Bro.Id), app.Services)];
                IEnumerable<SavedPlace> buildSavedPlaces =
                [
                    new SavedPlace() { SaveableId = _places[0].Id, SaverId = _bro.Id },
                    new SavedPlace() { SaveableId = _places[1].Id , SaverId = _bro.Id },
                    new SavedPlace() { SaveableId = _places[2].Id, SaverId = _bro.Id },
                ];

                await SavedPlaceFactory.CreateAsync(buildSavedPlaces, app.Services);

                _response = await app.GetAllSavedPlacesAsync(_bro.Id);
            }

            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task FetchesPlacesSavedByBroTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.Equal(_bro.Id, sp.SaverId),
                    sp => Assert.Equal(_bro.Id, sp.SaverId),
                    sp => Assert.Equal(_bro.Id, sp.SaverId));
            }

            [Fact]
            public async Task DoesNotFetchPlacesSavedByOtherBrosTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.NotEqual(app.Bro.Id, sp.SaverId),
                    sp => Assert.NotEqual(app.Bro.Id, sp.SaverId),
                    sp => Assert.NotEqual(app.Bro.Id, sp.SaverId));
            }

            [Fact]
            public async Task DoesNotFetchPlacesNotSavedByBroTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.NotEqual(_places[3].Id, sp.SaveableId),
                    sp => Assert.NotEqual(_places[3].Id, sp.SaveableId),
                    sp => Assert.NotEqual(_places[3].Id, sp.SaveableId));
            }

            private static IEnumerable<Place> BuildPlaces(Guid broId)
            {
                return
                [
                    new Place { Name = "Bookmarked by other bro", Address = "Address", BroId = broId },
                    new Place { Name = "Bookmarked by other bro", Address="Address", BroId = broId },
                    new Place { Name = "Bookmarked by other bro", Address = "Address", BroId = broId },
                    new Place { Name = "Not bookmarked by other bro", Address = "Address", BroId = broId },
                ];
            }
        }
    }
}
