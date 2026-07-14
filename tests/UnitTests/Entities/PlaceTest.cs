using Core.Dto;
using Core.PlaceAggregate;

namespace UnitTests.Entities;

public class PlaceTest
{
    [Fact]
    public void SetDetailsTest()
    {
        Place place = new() { Name = "Name", Address = "Address" };
        PlaceDetails details = new() { Name = "New Name", Address = "New Address" };

        place.SetDetails(details);

        Assert.Equal("New Name", place.Name);
        Assert.Equal("New Address", place.Address);
    }

    [Fact]
    public void AddPictureTest()
    {
        PlaceProfilePicture profilePicture = new() { Normal = "Normal" };

        Place place = new();

        place.AddPicture(profilePicture);

        Assert.NotEmpty(place.ProfilePictures);
        Assert.Single(place.ProfilePictures, p => p.Normal == "Normal");
    }


    [Fact]
    public void PictureTest()
    {
        PlaceProfilePicture profilePicture = new() { Normal = "Normal" };
        Place place = new();
        place.AddPicture(profilePicture);

        Assert.NotNull(place.Picture);
        Assert.IsType<PlaceProfilePicture>(place.Picture);
    }

    [Fact]
    public void AddPicturesTest()
    {
        Place place = new();
        List<PlaceExtraPicture> pictures = new List<PlaceExtraPicture>
        {
            new() { Normal = "Normal" },
            new() { Normal = "Normal 2" },
            new() { Normal = "Normal 3" },
        };

        place.AddPictures(pictures);

        Assert.Collection(place.Pictures,
            p => Assert.Equal("Normal", p.Normal),
            p => Assert.Equal("Normal 2", p.Normal),
            p => Assert.Equal("Normal 3", p.Normal));
    }
}