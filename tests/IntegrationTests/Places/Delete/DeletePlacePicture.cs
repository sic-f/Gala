using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Utility.Dto;
using Utility.Factories;
using Utility.Helpers;
using Utility.Scopes;

namespace IntegrationTests.Places.Delete;

public class DeletePlacePicture(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task DeletePlacePictureTestAync()
    {
        PlaceProfilePicture profilePicture = new()
        {
            Normal = ImageHelper.TestImageBPath,
            Small = ImageHelper.TestImageCPath,
        };
        PlaceAttributes attributes = new()
        {
            Name = "Name",
            Address = "Address",
            Picture = profilePicture,
            BroId = app.Bro.Id,
        };
        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app);

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
