using Core.PlaceAggregate;
using Infrastructure.Data;
using IntegrationTests.Scopes;
using Microsoft.Extensions.DependencyInjection;
using Ui.Dto;
using Utility.Dto;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test.Places.Update;

[Collection("Application Factory Collection")]
public class Upate(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task UpdateTestAsync()
    {
        PlaceAttributes attributes = new()
        {
            Name = "Test Name",
            Address = "Test Address",
            BroId = app.Bro.Id,
            Picture = new PlaceProfilePicture
            {
                Normal = ImageHelper.TestImageAPath,
                Small = ImageHelper.TestImageBPath,
                NormalLocal = ImageHelper.TestImageAPath,
                SmallLocal = ImageHelper.TestImageBPath
            },
        };

        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app.Services);

        PlaceDetails details = new()
        {
            Name = "Name",
            Address = "Address",
            Picture = ImageHelper.MockImageFormFile(),
        };

        await app.UpdatePlaceAsync(place.Id, details);

        Assert.True(app.Response.IsSuccessStatusCode);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        Place updatedPlace = await RepositoryHelper.GetPlaceByIdAsync(place.Id, context);
        await context.Entry(updatedPlace).Collection(p => p.ProfilePictures).LoadAsync();

        Assert.Single(updatedPlace.ProfilePictures);
    }
}
