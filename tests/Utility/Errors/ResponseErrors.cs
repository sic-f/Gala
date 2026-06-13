using System.Net.Http.Json;
using System.Text.Json;

namespace Utility.Errors;

public class ResponseErrors(ApplicationFactory<Program> app)
{
    public async Task<JsonElement> ErrorsResponseBody()
    {
        var responseBodyJsonElement = await ResponseBody();

        return responseBodyJsonElement.GetProperty("errors");
    }

    private async Task<JsonElement> ResponseBody()
    {
        return await app.Response.Content.ReadFromJsonAsync<JsonElement>();
    }
}