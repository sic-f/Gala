using Core.PlaceAggregate;
using IntegrationTests.Scopes;
using Utility.Dto;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test.PlaceProfilePictures;

[Collection("Application Factory Collection")]
public class Update(PlacePictureScope app) : BaseTest(app), IClassFixture<PlacePictureScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task UpdateCaptionTestAsync()
    {
        PlaceProfilePicture profilePicture = new()
        {
            Normal = "Normal",
            Small = "Small",
        };
        PlaceAttributes attributes = new()
        {
            Name = "Name",
            Address = "Address",
            BroId = app.Bro.Id,
            Picture = profilePicture,
        };
        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app.Services);

        await app.UpdatePictureCaptionAsync(place.Picture!.Id, caption: "New caption");

        PlaceProfilePicture updatedPlacePicture = await RepositoryHelper.GetPlacePictureByIdAsync(place.Picture!.Id, app.Services);

        Assert.True(app.Response.IsSuccessStatusCode);
        Assert.Equal("New caption", updatedPlacePicture.Caption);
    }
}
