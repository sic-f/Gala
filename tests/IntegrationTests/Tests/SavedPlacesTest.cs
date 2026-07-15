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
    private const int FetchLimit = 5;

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
            private IEnumerable<PlaceBookmark> _response = default!;
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
                
                _response = await app.GetAllSavedPlacesAsync(offset: GetFormattedDateTimeOffset(DateTimeOffset.UtcNow.AddSeconds(1)), limit: FetchLimit, _bro.Id);
            }

            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task DoesNotFetchPlacesNotSavedByBroTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.NotEqual("Unexpected", sp.Name),
                    sp => Assert.NotEqual("Unexpected", sp.Name),
                    sp => Assert.NotEqual("Unexpected", sp.Name));
            }

            [Fact]
            public async Task FetchesPlacesSavedByBroTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.Equal("Expected", sp.Name),
                    sp => Assert.Equal("Expected b", sp.Name),
                    sp => Assert.Equal("Expected c", sp.Name));
            }

            [Fact]
            public async Task FetchesPlacesWithPictureTestAsync()
            {
                Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
                Assert.Collection(_response,
                    sp => Assert.NotEqual("Small picture", sp.Picture),
                    sp => Assert.NotEqual("Small picture", sp.Picture),
                    sp => Assert.NotEqual("Small picture", sp.Picture));
            }

            private static IEnumerable<Place> BuildPlaces(Guid broId)
            {
                PlaceProfilePicture picture = new() { Small = "Small Picture " };
                var place = new Place { Name = "Expected", Address = "Address", BroId = broId };
                var placeB = new Place { Name = "Expected b", Address = "Address", BroId = broId };
                var placeC = new Place { Name = "Expected c", Address = "Address", BroId = broId };
                var placeD = new Place { Name = "Unexpected", Address = "Address", BroId = broId };

                place.AddPicture(picture);
                placeB.AddPicture(picture);
                placeC.AddPicture(picture);
                placeD.AddPicture(picture);

                return [place, placeB, placeC, placeD];
            }
        }

        [Collection("Application Factory Collection")]
        public class PaginatedTest(SavedScope app) : BaseTest(app), IClassFixture<SavedScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task FetchSavedPlacesPaginatedTestAsync()
            {
                Place[] places = BuildNewPlaces();
                await PlaceFactory.CreateAsync(places, app.Services);
                SavedPlace[] newSavedPlaces = BuildNewSavedPlaces(places);

                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.SavedPlace.AddRange(newSavedPlaces);
                await context.SaveChangesAsync();


                DateTimeOffset createdAt = DateTimeOffset.UtcNow.AddSeconds(30);
                IEnumerable<PlaceBookmark> bookmarks = await app.GetAllSavedPlacesAsync(GetFormattedDateTimeOffset(createdAt), FetchLimit, app.Bro.Id);

                Assert.Collection(bookmarks,
                    p => Assert.Equal(places[12].Id, p.PlaceId),
                    p => Assert.Equal(places[13].Id, p.PlaceId),
                    p => Assert.Equal(places[11].Id, p.PlaceId),
                    p => Assert.Equal(places[14].Id, p.PlaceId),
                    p => Assert.Equal(places[10].Id, p.PlaceId));

                createdAt = newSavedPlaces[5].CreatedAt;
                bookmarks = await app.GetAllSavedPlacesAsync(GetFormattedDateTimeOffset(createdAt), FetchLimit, app.Bro.Id);

                Assert.Collection(bookmarks,
                    p => Assert.Equal(places[4].Id, p.PlaceId),
                    p => Assert.Equal(places[3].Id, p.PlaceId),
                    p => Assert.Equal(places[2].Id, p.PlaceId),
                    p => Assert.Equal(places[1].Id, p.PlaceId),
                    p => Assert.Equal(places[0].Id, p.PlaceId));

                createdAt = newSavedPlaces[10].CreatedAt;
                bookmarks = await app.GetAllSavedPlacesAsync(GetFormattedDateTimeOffset(createdAt), FetchLimit, app.Bro.Id);

                Assert.Collection(bookmarks,
                    p => Assert.Equal(places[9].Id, p.PlaceId),
                    p => Assert.Equal(places[8].Id, p.PlaceId),
                    p => Assert.Equal(places[7].Id, p.PlaceId),
                    p => Assert.Equal(places[6].Id, p.PlaceId),
                    p => Assert.Equal(places[5].Id, p.PlaceId));
            }

            private SavedPlace[] BuildNewSavedPlaces(Place[] places)
            {
                return 
                [
                    new SavedPlace
                    {
                        SaveableId = places[0].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[1].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[2].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(5),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[3].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(7),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[4].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(9),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[5].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(11),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[6].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(13),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[7].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(15),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[8].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(17),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[9].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(19),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[10].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(21),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[14].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(23),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[11].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(25),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[13].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(27),
                    },
                    new SavedPlace
                    {
                        SaveableId = places[12].Id,
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(29),
                    },
                ];
            }

            private Place[] BuildNewPlaces()
            {
                return
                [
                    new Place
                    {
                        Name = "1st Batch A",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    },
                    new Place
                    {
                        Name = "1st Batch B",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3),
                    },
                    new Place
                    {
                        Name = "1st Batch C",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(5),
                    },
                    new Place
                    {
                        Name = "1st Batch D",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(7),
                    },
                    new Place
                    {
                        Name = "1st Batch E",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(9),
                    },
                    new Place
                    {
                        Name = "2nd Batch A",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(11),
                    },
                    new Place
                    {
                        Name = "2nd Batch B",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(13),
                    },
                    new Place
                    {
                        Name = "2nd Batch C",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(15),
                    },
                    new Place
                    {
                        Name = "2nd Batch D",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(17),
                    },
                    new Place
                    {
                        Name = "2nd Batch E",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(19),
                    },
                    new Place
                    {
                        Name = "3rd Batch A",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(21),
                    },
                    new Place
                    {
                        Name = "3rd Batch B",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(23),
                    },
                    new Place
                    {
                        Name = "3rd Batch C",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(25),
                    },
                    new Place
                    {
                        Name = "3rd Batch D",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(27),
                    },
                    new Place
                    {
                        Name = "3rd Batch E",
                        Address = string.Empty,
                        BroId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(29),
                    },
                ];
            }
        }

        private static string GetFormattedDateTimeOffset(DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.sssssssZ");
        }
    }
}
