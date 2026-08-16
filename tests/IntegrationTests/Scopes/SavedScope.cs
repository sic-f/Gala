using Core.Dto;
using Core.SavedAggregate;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class SavedScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => $"api/Saveds";

    internal async Task<IEnumerable<SavedDto>> FetchSavedsAsync(string offset, int limit)
    {
        return (await Client.GetFromJsonAsync<IEnumerable<SavedDto>>($"{DefaultStartUrl}/?offset={offset}&limit={limit}"))!;
    }

    internal async Task<IEnumerable<SavedDto>> GetAllSavedPlacesAsync(string offset, int limit)
    {
        return (await Client.GetFromJsonAsync<IEnumerable<SavedDto>>($"{DefaultStartUrl}?offset={offset}&limit={limit}"))!;
    }

    internal async Task SavePlaceAsync(Guid saveableId)
    {
        Response = await Client.PostAsJsonAsync(DefaultStartUrl, new { saveableId });
    }

    internal async Task<SavedPlace> SavePlaceSuccessResponseAsync()
    {
        return (await Response.Content.ReadFromJsonAsync<SavedPlace>())!;
    }
}
