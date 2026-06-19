using Core.Interfaces;
using Core.PlaceAggregate;

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

        IPlaceDetailer details = new() { Name = "Updated", Address = "Updated" };

        place.UpdateDetails(details);

        Assert.Equal("Updated", place.Name);
        Assert.Equal("Updated", place.Address);
        Assert.NotEqual("Updated", place.Description);
    }
}
