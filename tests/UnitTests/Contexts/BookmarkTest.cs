using Context;
using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using Moq;

namespace UnitTests.Contexts;

public class BookmarkTest()
{
    [Fact]
    public async Task InvalidWhenSavingOwnPlaceTestAsync()
    {
        Bro bro = new() { Id = Guid.NewGuid() };
        Place place = new() { BroId = bro.Id };

        var context = new Mock<ApplicationDbContext>();
        var savedRepository = new Mock<ISavedRepository>();
        var placeRepository = new Mock<IPlaceRepository>();
        placeRepository.Setup(repository => repository.FetchPlaceAsync(It.IsAny<Guid>()).Result).Returns(place);
        Bookmark bookmark = new(savedRepository.Object, placeRepository.Object);

        Exception exception = await Record.ExceptionAsync(() => bookmark.SavePlaceAsync(place.Id, bro.Id));

        Assert.IsType<SaveOwnPlaceException>(exception);
    }
}
