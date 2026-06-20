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
}