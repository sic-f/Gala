using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class SavedScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => $"api/Saves";

    internal async Task SavePlaceAsync(Guid id)
    {
        Response = await Client.PostAsJsonAsync(DefaultStartUrl, id);
    }
}
