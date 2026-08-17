using Core.Dto;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class SavedPlaceScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Saved-Places";

    internal async Task<IEnumerable<SavedDto>> FetchSavedPlacesAsync(string offset, int limit)
    {
        return (await Client.GetFromJsonAsync<IEnumerable<SavedDto>>($"{DefaultStartUrl}?offset={offset}&limit={limit}"))!;
    }
}
