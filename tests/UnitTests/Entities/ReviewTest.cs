using Core.Dto;
using Core.PlaceAggregate;
using Infrastructure.Identity;

namespace UnitTests.Entities;

public class ReviewTest
{
    [Fact]
    public void SetDetailsTest()
    {
        var bro = new Bro { Id = Guid.NewGuid() };
        var place = new Place { Id = Guid.NewGuid() };

        var details = new ReviewDetails { Content = "Content", PlaceId = place.Id };

        var review = new Review();

        review.SetDetails(details, bro.Id);

        Assert.Equal("Content", review.Content);
        Assert.Equal(place.Id, review.PlaceId);
        Assert.Equal(bro.Id, review.ReviewerId);
    }
}
