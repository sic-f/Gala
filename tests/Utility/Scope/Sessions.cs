using System.Net.Http.Json;
using static Core.Dto.Session;

namespace Utility.Scope;

public class Sessions() : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Sessions";

    public async Task RefreshTokenAsync(string token)
    {
        Response = await Client.PostAsJsonAsync<string>($"{DefaultStartUrl}/Refresh", token);
    }

    public async Task<RefreshTokenResponse?> RefreshTokenResponseAsync()
    {
        return (await Response.Content.ReadFromJsonAsync<RefreshTokenResponse>());
    }
}
