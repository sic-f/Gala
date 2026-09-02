using Core.Dto;
using Core.Entities.Comment;
using Core.PlaceAggregate;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;
using Utility.Helpers;

namespace IntegrationTests.Test.Comments;

[Collection("Application Factory Collection")]
public class FetchCommentsTest(CommentScope app) : BaseTest(app), IClassFixture<CommentScope>
{
    private const int FetchLimit = 5;
    private IEnumerable<CommentDto> _response = default!;

    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        Place place = await PlaceFactory.CreateCompleteDetails(app.BroId, app.Services);
        BroWithFullName broDetails = new("user@example.me", "Hey@123", "Martin", "Nievera");
        Bro otherBro = await BroFactory.CreateAsync(broDetails, app.Services);
        IEnumerable<PlacePictureComment> buildComments = BuildComments(
            placePictureId: place.Picture!.Id,
            broId: app.BroId,
            otherBroId: otherBro.Id);
        await CommentFactory.CreateAsync(buildComments, app.Services);
        _response = (await app.SuccessGetCommentsAsync(
            offset: DateHelper.GetFormattedDateTimeOffset(DateTimeOffset.UtcNow.AddSeconds(5)),
            limit: FetchLimit,
            place.Picture!.Id))!;
        Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
    }

    [Fact]
    public async Task BodyTestAsync()
    {
        Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
        Assert.Collection(_response,
            comment => Assert.Equal("Comment Body B", comment.Body),
            comment => Assert.Equal("Comment Body A", comment.Body));
    }

    [Fact]
    public async Task CommenterNameTestAsync()
    {
        Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
        Assert.Collection(_response,
            comment => Assert.Equal("Martin Nievera", comment.CommenterName),
            comment => Assert.Equal("test@example.net", comment.CommenterName));
    }

    [Fact]
    public async Task CommenterAvatarTestAsync()
    {
        Assert.Collection(_response,
            comment => Assert.Null(comment.CommenterAvatar),
            comment => Assert.Null(comment.CommenterAvatar));
    }

    private static IEnumerable<PlacePictureComment> BuildComments(Guid placePictureId, Guid broId, Guid otherBroId)
    {
        return [
            new()
            {
                Body = "Comment Body A",
                CommentableId = placePictureId,
                CommenterId = broId,
                CreatedAt = DateTime.UtcNow.AddSeconds(1),
            },
            new()
            {
                Body = "Comment Body B",
                CommentableId = placePictureId,
                CommenterId = otherBroId,
                CreatedAt = DateTime.UtcNow.AddSeconds(3),
            }
        ];
    }
}
