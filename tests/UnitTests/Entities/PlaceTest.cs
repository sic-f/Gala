using Core.PlaceAggregate;
using Ui.Dto;

namespace UnitTests.Entities;

public class PlaceTest
{
    [Fact]
    public async Task UpdateDetailsTest()
    {
        Place place = new()
        {
            Name = "Test",
            Address = "Test Address",
            Description = "Test Description",
        };

        PlaceDetails details = new() { Name = "Updated", Address = "Updated" };

        place.UpdateDetails(details);

        Assert.Equal("Updated", place.Name);
        Assert.Equal("Updated", place.Address);
        Assert.NotEqual("Updated", place.Description);
    }

    [Fact]
    public void AddProfilePictureTest()
    {
        PlaceProfilePicture profilePicture = new() { Normal = "Normal" };

        Place place = new();

        place.AddPicture(profilePicture);

        Assert.NotEmpty(place.ProfilePictures);
        Assert.Single(place.ProfilePictures, p => p.Normal == "Normal");
    }


    [Fact]
    public async Task PictureTest()
    {
        PlaceProfilePicture profilePicture = new() { Normal = "Normal" };
        Place place = new();
        place.AddPicture(profilePicture);

        Assert.NotNull(place.Picture);
        Assert.IsType<PlaceProfilePicture>(place.Picture);
    }
}