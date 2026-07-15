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

                
                _response = await app.GetAllSavedPlacesAsync(offset: DateTimeOffset.UtcNow.ToString(), limit: FetchLimit, _bro.Id);
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
                SavedPlace[] newSavedPlaces = BuildNewSavedPlaces();

                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.SavedPlace.AddRange(newSavedPlaces);
                await context.SaveChangesAsync();

                DateTimeOffset createdAt = newSavedPlaces[6].CreatedAt;
                IEnumerable<PlaceBookmark>? bookmarks = await app.GetAllSavedPlacesAsync(GetFormattedDateTimeOffset(createdAt), FetchLimit, app.Bro.Id);

                Assert.Collection(bookmarks!,
                    p => Assert.Equal("1st Batch E", p.Name),
                    p => Assert.Equal("1st Batch D", p.Name),
                    p => Assert.Equal("1st Batch C", p.Name),
                    p => Assert.Equal("1st Batch B", p.Name),
                    p => Assert.Equal("1st Batch A", p.Name));

                createdAt = newSavedPlaces[11].CreatedAt;
                bookmarks = await app.GetAllSavedPlacesAsync(GetFormattedDateTimeOffset(createdAt), FetchLimit, app.Bro.Id);

                Assert.Collection(bookmarks!,
                    p => Assert.Equal("2nd Batch E", p.Name),
                    p => Assert.Equal("2nd Batch D", p.Name),
                    p => Assert.Equal("2nd Batch C", p.Name),
                    p => Assert.Equal("2nd Batch B", p.Name),
                    p => Assert.Equal("2nd Batch A", p.Name));
            }

            private SavedPlace[] BuildNewSavedPlaces()
            {
                return 
                [
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(5),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(7),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(9),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(11),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(13),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(15),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(17),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(19),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(21),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(23),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(25),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(27),
                    },
                    new SavedPlace
                    {
                        SaverId = app.Bro.Id,
                        CreatedAt = DateTimeOffset.UtcNow.AddSeconds(29),
                    },
                ];
            }

            private static string GetFormattedDateTimeOffset(DateTimeOffset dateTime)
            {
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.sssssssZ");
            }
        }

    }
}
