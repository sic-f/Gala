using Core.PlaceAggregate;
using Infrastructure.Data;
using IntegrationTests.Scopes;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;
using Utility.Factories;

namespace IntegrationTests.Test.Places;

public class FetchPlaces()
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
    public class FetchPlace(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
    {
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task FetchPlaceTestAsync()
        {
            PlaceAttributes attributes = new() { Name = "Name Test", Address = "Address Test", BroId = app.Bro.Id };
            Place place = await PlaceFactory.CreatePlaceAsync(attributes, app.Services);

            Core.Dto.PlaceDetails details = await app.GetPlaceAsync(place.Id);

            Assert.Equal("Name Test", details.Name);
            Assert.Equal("Address Test", details.Address);
            Assert.Null(details.BookmarkId);
        }
    }
}