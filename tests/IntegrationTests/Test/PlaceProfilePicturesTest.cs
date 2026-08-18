using Core.Dto;
using Core.PlaceAggregate;
using IntegrationTests.Helpers;
using IntegrationTests.Scopes;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test;

public class PlaceProfilePicturesTest
{
    [Collection("Application Factory Collection")]
    public class DetailsTest(PlacePictureScope app) : BaseTest(app), IClassFixture<PlacePictureScope>
    {
        public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

        [Fact]
        public async Task GetPlacePictureTestAsync()
        {
            Place place = await PlaceFactory.CreateAsync(BuildPlace(), app.Services);

            PlacePictureDto placePicture = await app.GetPlaceProfilePictureAsync(place.Picture!.Id);

            Assert.True(app.Response.IsSuccessStatusCode);
            Assert.Equal(ImageHelper.TestImageBPath, placePicture.Normal);
            Assert.Equal("Test Caption", placePicture.Caption);
            Assert.Equal(place.Id, placePicture.PlaceId);

        }

        private Place BuildPlace()
        {
            PlaceProfilePicture picture = new()
            {
                Caption = "Test Caption",
                Normal = ImageHelper.TestImageBPath,
                Small = ImageHelper.TestImageCPath,
            };
            Place place = new()
            {
                Name = "Test Name",
                Address = "Test Adress",
                BroId = app.BroId,
            };

            place.AddPicture(picture);

            return place;
        }
    }

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
            Place newPlace = new()
            {
                Name = "Name",
                Address = "Address",
                BroId = app.BroId,
            };
            newPlace.AddPicture(profilePicture);
            Place place = await PlaceFactory.CreateAsync(newPlace, app.Services);

            await app.UpdatePictureCaptionAsync(place.Picture!.Id, caption: "New caption");

            PlaceProfilePicture updatedPlacePicture = await RepositoryHelper.GetPlacePictureByIdAsync(place.Picture!.Id, app.Services);

            Assert.True(app.Response.IsSuccessStatusCode);
            Assert.Equal("New caption", updatedPlacePicture.Caption);
        }
    }
}
