using Core.Dto;
using Core.PlaceAggregate;
using IntegrationTests.Scopes;
using Utility.Dto;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test.PlaceProfilePictures;

[Collection("Application Factory Collection")]
public class Details(PlacePictureScope app) : BaseTest(app), IClassFixture<PlacePictureScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task GetPlacePictureTestAsync()
    {
        PlaceAttributes attributes = new()
        {
            Name = "Test Name",
            Address = "Test Adress",
            BroId = app.Bro.Id,
            Picture = new PlaceProfilePicture
            {
                Caption = "Test Caption",
                Normal = ImageHelper.TestImageBPath,
                Small = ImageHelper.TestImageCPath,
            }
        };
        Place place = await PlaceFactory.CreatePlaceAsync(attributes, app.Services);

        PlacePictureDto placePicture = await app.GetPlaceProfilePictureAsync(place.Picture!.Id);

        Assert.True(app.Response.IsSuccessStatusCode);
        Assert.Equal(ImageHelper.TestImageBPath, placePicture.Normal);
        Assert.Equal("Test Caption", placePicture.Caption);
        Assert.Equal(place.Id, placePicture.PlaceId);
    }
}
