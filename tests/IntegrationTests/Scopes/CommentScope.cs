using Core.Dto;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class CommentScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Place-Pictures";

    internal async Task<IEnumerable<CommentDto>?> SuccessGetCommentsAsync(Guid placePictureId)
    {
        return await Client.GetFromJsonAsync<IEnumerable<CommentDto>>($"{DefaultStartUrl}/{placePictureId}/comments");
    }

    internal async Task<CommentDto?> SuccessWriteCommentResponseAsync()
    {
        return await Response.Content.ReadFromJsonAsync<CommentDto>();
    }

    internal async Task WriteCommentAsync(Guid pictureId, CommentDetails body)
    {
        Response = await Client.PostAsJsonAsync<CommentDetails>($"{DefaultStartUrl}/{pictureId}/comments", body);
    }
}
