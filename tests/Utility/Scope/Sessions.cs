using System.Net.Http.Json;
using static Core.Dto.Session;

namespace Utility.Scope;

public class Sessions : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Sessions";

    public async Task RefreshTokenAsync(RefreshTokenables tokenables)
    {
        Response = await Client.PostAsJsonAsync<RefreshTokenables>($"{DefaultStartUrl}/Refresh", tokenables);
    }

    public async Task<RefreshTokenResponse?> RefreshTokenResponseAsync()
    {
        return (await Response.Content.ReadFromJsonAsync<RefreshTokenResponse>());
    }
}
