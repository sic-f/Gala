using Core.PlaceAggregate;
using Core.Dto;
using IntegrationTests.Scopes;
using System.Net;
using Utility.Factories;

namespace IntegrationTests.Test.Comments;

[Collection("Application Factory Collection")]
public class WriteCommentTest(CommentScope app) : BaseTest(app), IClassFixture<CommentScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task Returns422WhenBlankBodyAsyncTest()
    {
        Place place = await PlaceFactory.CreateCompleteDetails(app.BroId, app.Services);
        Guid pictureId = place.Picture!.Id;
        CommentDetails details = new(Body: string.Empty);

        await app.WriteCommentAsync(pictureId, details);

        Assert.Equal(HttpStatusCode.UnprocessableContent, app.ResponseStatusCode);
    }
}
