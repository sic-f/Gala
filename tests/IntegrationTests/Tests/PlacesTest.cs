using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using IntegrationTests.Errors;
using IntegrationTests.Helpers;
using IntegrationTests.Scopes;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ui.Dto;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Tests;

public class PlacesTest
{
    public class SubmitTest
    {
        [Collection("Application Factory Collection")]
        public class InvalidLengthTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
        {
            private readonly PlaceErrors _errors = new(app);
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task LongValuesTestAsync()
            {
                await WhenBroSubmitsPlaceWithInvalidLongValuesAsync();
                ThenPlaceWillHaveErrorsAndNotSubmitted();
            }

            private async Task WhenBroSubmitsPlaceWithInvalidLongValuesAsync()
            {
                var invalidValues = new LongInvalidPlaceValues();
                PlaceDetails details = new()
                {
                    Name = invalidValues.Name,
                    Description = invalidValues.Description,
                    Information = invalidValues.Information,
                    Address = invalidValues.Address,
                    Mobile = invalidValues.Mobile,
                    Landline = invalidValues.Landline,
                    Email = invalidValues.Email,
                    Website = invalidValues.Website,
                    Facebook = invalidValues.Facebook,
                    Instagram = invalidValues.Instagram
                };

                await app.SubmitPlaceWithLongInvalidValuesAsync(details);
            }

            private void ThenPlaceWillHaveErrorsAndNotSubmitted()
            {
                Assert.Equal("Exceeds 75 characters", _errors.Name);
                Assert.Equal("Exceeds 155 characters", _errors.Description);
                Assert.Equal("Exceeds 50,000 characters", _errors.Information);
                Assert.Equal("Exceeds 95 characters", _errors.Address);
                Assert.Equal("Exceeds 15 digits", _errors.Mobile);
                Assert.Equal("Exceeds 15 digits", _errors.Landline);
                Assert.Equal("Exceeds 254 characters", _errors.Email);
                Assert.Equal("Exceeds 2,083 characters", _errors.Website);
                Assert.Equal("Exceeds 2,083 characters", _errors.Facebook);
                Assert.Equal("Exceeds 30 characters", _errors.Instagram);
            }
        }

        [Collection("Application Factory Collection")]
        public class PublicAccessTest(PlaceScope app) : BaseTest(app, isAuthenticated: false), IClassFixture<PlaceScope>
        {
            [Fact]
            public async Task UnauthenticatedOrUnregisteredVisitorTest()
            {
                await WhenBroSubmitsPlace();
                ThenPlaceWillNotBeCreated();
            }

            private async Task WhenBroSubmitsPlace()
            {
                PlaceDetails placeDetails = new() { Name = string.Empty, Address = string.Empty };
                await app.SubmitPlaceUnauthenticatedAsync(placeDetails);
            }

            private void ThenPlaceWillNotBeCreated()
            {
                Assert.Equal(HttpStatusCode.Unauthorized, app.ResponseStatusCode);
            }
        }

        [Collection("Application Factory Collection")]
        public class RequiredDetailsBlankTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
        {
            private readonly PlaceErrors _errors = new(app);
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task RequiredDetailsBlankTestAsync()
            {
                await WhenUserSubmitsWithRequiredDetailsNotPresent();
                ThenAllRequiredFieldsForPlaceWillHaveErrorMessages();
            }

            private async Task WhenUserSubmitsWithRequiredDetailsNotPresent()
            {
                PlaceDetails placeDetails = new() { Name = string.Empty, Address = string.Empty };

                await app.SubmitRequiredDetailsBlankAsync(placeDetails);
            }

            private void ThenAllRequiredFieldsForPlaceWillHaveErrorMessages()
            {
                Assert.Equal("Name of place is required", _errors.Name);
                Assert.Equal("Address is required", _errors.Address);
            }
        }
    }

    public class FetchTest
    {
        [Collection("Application Factory Collection")]
        public class PaginatedTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task FetchPlacesTestAsync()
            {
                IEnumerable<Place> newPlaces = BuildNewPlaces();

                using var scope = app.Services.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<ApplicationDbContext>();

                context.Place.AddRange(newPlaces);
                await context.SaveChangesAsync();

                DateTimeOffset? createdAt = context.Place.FirstOrDefault<Place>(p => p.Name == "2nd Batch A")?.CreatedAt;
                var fetchLimit = 5;
                IEnumerable<Place>? places = await app.GetPlacesAsync(GetFormattedDateTimeOffset(createdAt), fetchLimit);

                Assert.Collection(places!,
                    p => Assert.Equal("1st Batch E", p.Name),
                    p => Assert.Equal("1st Batch D", p.Name),
                    p => Assert.Equal("1st Batch C", p.Name),
                    p => Assert.Equal("1st Batch B", p.Name),
                    p => Assert.Equal("1st Batch A", p.Name));

                createdAt = context.Place.FirstOrDefault<Place>(p => p.Name == "3rd Batch A")?.CreatedAt;
                places = await app.GetPlacesAsync(GetFormattedDateTimeOffset(createdAt), fetchLimit);

                Assert.Collection(places!,
                    p => Assert.Equal("2nd Batch E", p.Name),
                    p => Assert.Equal("2nd Batch D", p.Name),
                    p => Assert.Equal("2nd Batch C", p.Name),
                    p => Assert.Equal("2nd Batch B", p.Name),
                    p => Assert.Equal("2nd Batch A", p.Name));
            }

            private IEnumerable<Place> BuildNewPlaces()
            {
                return [
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

            private static string? GetFormattedDateTimeOffset(DateTimeOffset? dateTime)
            {
                return dateTime?.ToString("yyyy-MM-ddTHH:mm:ss.sssssssZ");
            }
        }

        [Collection("Application Factory Collection")]
        public class FetchPlaceTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task FetchPlaceTestAsync()
            {
                Place newPlace = new() { Name = "Name Test", Address = "Address Test", BroId = app.Bro.Id };
                Place place = await PlaceFactory.CreateAsync(newPlace, app.Services);

                Core.Dto.PlaceDetails details = await app.GetPlaceAsync(place.Id);

                Assert.Equal("Name Test", details.Name);
                Assert.Equal("Address Test", details.Address);
                Assert.Null(details.BookmarkId);
            }
        }
    }

    public class UpdateTest
    {
        [Collection("Application Factory Collection")]
        public class ExclusiveUpdateTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task ExclusivePlaceUpdateTestAsync()
            {
                Place place = await GivenPlaceOwnedByAnotherBroAsync();
                await WhenBroTriesToUpdatePlaceOwnedByAnotherBroAsync(place.Id);
                await ThenBroWillNotBeAbleToUpdateThePlace(place.Id);
            }

            private async Task<Place> GivenPlaceOwnedByAnotherBroAsync()
            {
                Bro placeOwner = await BroFactory.CreateAsync("test@example.org", "Hey@123", app.Services);
                Place newPlace = new()
                {
                    Name = "Test Name",
                    Address = "Test Address",
                    BroId = placeOwner.Id,
                };
                Place place = await PlaceFactory.CreateAsync(newPlace, app.Services);

                return place;
            }

            private async Task WhenBroTriesToUpdatePlaceOwnedByAnotherBroAsync(Guid placeId)
            {
                PlaceDetails updateAttributes = new()
                {
                    Name = "Test Name Update",
                    Address = "Test Address Update",
                    Picture = ImageHelper.MockImageFormFile()
                };

                await app.UpdatePlaceAsync(placeId, updateAttributes);
            }

            private async Task ThenBroWillNotBeAbleToUpdateThePlace(Guid placeId)
            {
                Assert.Equal(
                    "❌👦🏣 You did not submit this place and is not the owner, you are not allowed to modify it.",
                    (await app.Response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("detail").ToString());
                var exception = Record.Exception(() => app.Response.EnsureSuccessStatusCode());
                Assert.NotNull(exception);

                var scope = app.Services.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                Place place = db.Place.First(p => p.Id == placeId);

                Assert.NotEqual("Test Name Update", place.Name);
                Assert.NotEqual("Test Address Update", place.Address);
            }
        }

        [Collection("Application Factory Collection")]
        public class UpdateDetailsTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task UpdateTestAsync()
            {
                PlaceProfilePicture picture = new()
                {
                    Normal = ImageHelper.TestImageAPath,
                    Small = ImageHelper.TestImageBPath,
                    NormalLocal = ImageHelper.TestImageAPath,
                    SmallLocal = ImageHelper.TestImageBPath,
                };
                Place newPlace = new()
                {
                    Name = "Test Name",
                    Address = "Test Address",
                    BroId = app.Bro.Id,
                };
                newPlace.AddPicture(picture);

                Place place = await PlaceFactory.CreateAsync(newPlace, app.Services);

                PlaceDetails details = new()
                {
                    Name = "Name",
                    Address = "Address",
                    Picture = ImageHelper.MockImageFormFile(),
                };

                await app.UpdatePlaceAsync(place.Id, details);

                Assert.True(app.Response.IsSuccessStatusCode);

                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Place updatedPlace = await RepositoryHelper.GetPlaceByIdAsync(place.Id, context);
                await context.Entry(updatedPlace).Collection(p => p.ProfilePictures).LoadAsync();

                Assert.Single(updatedPlace.ProfilePictures);
            }
        }
    }

    public class DeleteTest
    {
        [Collection("Application Factory Collection")]
        public class PublicAccessTest(PlaceScope app) : BaseTest(app, isAuthenticated: false), IClassFixture<PlaceScope>
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
                Bro bro = await BroFactory.CreateAsync("test@example.net", "Hey@123", app.Services);
                Place place = new() { Name = "Test Name", Address = "Test Adress", BroId = bro.Id };

                return await PlaceFactory.CreateAsync(place, app.Services);
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

        [Collection("Application Factory Collection")]
        public class DeletePlaceTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
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
                Place place = new() { Name = "Test Name", Address = "Test Address", BroId = app.Bro.Id };

                return await PlaceFactory.CreateAsync(place, app.Services);
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
                var exception = await Record.ExceptionAsync(async () => await app.DeletePlaceAsync(placeId));
                Assert.NotNull(exception);
                Assert.Equal("Sequence contains no elements.", exception.Message);
            }

            private void ThenPlaceIsNotDeleted(Guid id)
            {
                using var scope = app.Services.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<ApplicationDbContext>();

                var dbException = Record.Exception(() => context.Place.First(p => p.Id == id));
                Assert.Null(dbException);
            }
        }

        [Collection("Application Factory Collection")]
        public class DeletePlacePictureTest(PlacePictureScope app) : BaseTest(app), IClassFixture<PlacePictureScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task DeletePlacePictureTestAync()
            {
                PlaceProfilePicture picture = new()
                {
                    Normal = "Normal",
                    Small = "Small",
                };
                Place newPlace = new()
                {
                    Name = "Name",
                    Address = "Address",
                    BroId = app.Bro.Id,
                };
                newPlace.AddPicture(picture);

                Place place = await PlaceFactory.CreateAsync(newPlace, app.Services);

                await app.DeletePictureAsync(place.Picture!.Id);

                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var updatedPlace = await RepositoryHelper.GetPlaceByIdAsync(place.Id, context);

                await context.Entry(updatedPlace).Collection(p => p.ProfilePictures).LoadAsync();

                Assert.True(app.Response.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NoContent, app.ResponseStatusCode);
                Assert.Empty(updatedPlace.ProfilePictures);
            }
        }

        [Collection("Application Factory Collection")]
        public class ExclusiveDeleteTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
        {
            public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

            [Fact]
            public async Task PlaceOwnerExclusiveDeleteTestAsync()
            {
                Place place = await GivenPlaceAndBroNotThePlaceOwnerAsync();
                await WhenPlaceIsDeletedByNotTheOwnerAsync(place.Id);
                ThenPlaceWillNotBeDeleted(place.Id);
            }

            private async Task<Place> GivenPlaceAndBroNotThePlaceOwnerAsync()
            {
                Bro placeOwner = await BroFactory.CreateAsync("test@example.org", "Hey@123", app.Services);
                Place place = new() { Name = "Test Name", Address = "Test Address", BroId = placeOwner.Id };

                return await PlaceFactory.CreateAsync(place, app.Services);
            }

            private async Task WhenPlaceIsDeletedByNotTheOwnerAsync(Guid placeId)
            {
                await app.DeletePlaceAsync(placeId);
            }

            private void ThenPlaceWillNotBeDeleted(Guid placeId)
            {
                var exception = Record.Exception(() => app.Response.EnsureSuccessStatusCode());
                Assert.NotNull(exception);

                using var scope = app.Services.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<ApplicationDbContext>();

                var dbException = Record.Exception(() => context.Place.First(p => p.Id == placeId));
                Assert.Null(dbException);
            }
        }
    }
}