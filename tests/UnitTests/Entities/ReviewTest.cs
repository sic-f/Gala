using Core.Dto;
using Core.PlaceAggregate;
using Infrastructure.Identity;
using Utility.Factories;

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

    [Fact]
    public void UpdateDetailsTest()
    {
        DateTimeOffset initialLastModified = DateTimeOffset.UtcNow;
        var review = new Review { Headline = "Headline", Content = "Content", LastModified = initialLastModified };
        var details = new ReviewDetails { Headline = "New Headline", Content = "New Content" };

        review.UpdateDetails(details);

        Assert.Equal("New Headline", review.Headline);
        Assert.Equal("New Content", review.Content);
        Assert.NotEqual(initialLastModified, review.LastModified);
    }
}
