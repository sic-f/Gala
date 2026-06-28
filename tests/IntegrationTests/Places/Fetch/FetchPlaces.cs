using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Utility.Scopes;

namespace IntegrationTests.Places.Fetch;

public class FetchPlaces(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
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

        DateTimeOffset? lastModifiedDate = context.Place.FirstOrDefault<Place>(p => p.Name == "2nd Batch E")?.LastModified;
        var fetchLimit = 5;
        IEnumerable<Place>? places = await app.GetPlacesAsync(lastModifiedDate, fetchLimit);

        Assert.Collection(places!,
            p => Assert.Equal("3rd Batch E", p.Name),
            p => Assert.Equal("3rd Batch D", p.Name),
            p => Assert.Equal("3rd Batch C", p.Name),
            p => Assert.Equal("3rd Batch B", p.Name),
            p => Assert.Equal("3rd Batch A", p.Name));

        lastModifiedDate = context.Place.FirstOrDefault<Place>(p => p.Name == "1st Batch E")?.LastModified;
        places = await app.GetPlacesAsync(lastModifiedDate, fetchLimit);

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
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "1st Batch B",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "1st Batch C",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "1st Batch D",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "1st Batch E",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "2nd Batch A",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "2nd Batch B",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "2nd Batch C",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "2nd Batch D",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "2nd Batch E",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "3rd Batch A",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "3rd Batch B",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "3rd Batch C",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "3rd Batch D",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
            new Place
            {
                Name = "3rd Batch E",
                Address = string.Empty,
                BroId = app.Bro.Id,
                LastModified = DateTimeOffset.UtcNow,
            },
        ];
    }

    private static IEnumerable<Place> GetPlaces(ApplicationDbContext context, DateTimeOffset? lastModifiedDate)
    {
        return [.. context.Place.Where(p => p.LastModified > lastModifiedDate)
                                .OrderBy(p => p.LastModified)
                                .Take(5)
                                .OrderDescending()];
    }
}
