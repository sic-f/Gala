using Core.Dto;
using Core.PlaceAggregate;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;

namespace IntegrationTests.Test.Comments;

[Collection("Application Factory Collection")]
public class WriteCommentTest(CommentScope app) : BaseTest(app), IClassFixture<CommentScope>
{
    private Guid _pictureId;
    private CommentDetails _details = default!;

    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        Place place = await PlaceFactory.CreateCompleteDetails(app.BroId, app.Services);
        _pictureId = place.Picture!.Id;
        _details = new() { Body = string.Empty };
    }

    [Fact]
    public async Task Returns422WithBlankBodyAsyncTest()
    {
        await app.WriteCommentAsync(_pictureId, _details);

        Assert.Equal(HttpStatusCode.BadRequest, app.ResponseStatusCode);
    }

    [Fact]
    public async Task ReturnsCreatedWithBodyTestAsync()
    {
        _details.Body = "Comment Body";
        await app.WriteCommentAsync(_pictureId, _details);

        Assert.Equal(HttpStatusCode.Created, app.ResponseStatusCode);
    }
}
