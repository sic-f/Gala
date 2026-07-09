using Core.PlaceAggregate;
using Utility.Dto;
using Utility.Factories;
using Utility.Helpers;
using Utility.Scopes;

namespace IntegrationTests.PlaceProfilePictures;

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
        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app);

        await app.UpdatePictureCaptionAsync(place.Picture!.Id, caption: "New caption");

        PlaceProfilePicture updatedPlacePicture = await RepositoryHelper.GetPlacePictureByIdAsync(place.Picture!.Id, app);

        Assert.True(app.Response.IsSuccessStatusCode);
        Assert.Equal("New caption", updatedPlacePicture.Caption);
    }
}
