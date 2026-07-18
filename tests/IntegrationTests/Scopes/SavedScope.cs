using Core.Dto;
using Core.SavedAggregate;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class SavedScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => $"api/Saves";

    internal async Task<IEnumerable<PlaceBookmark>> GetAllSavedPlacesAsync(string offset, int limit)
    {
        return (await Client.GetFromJsonAsync<IEnumerable<PlaceBookmark>>($"{DefaultStartUrl}?offset={offset}&limit={limit}"))!;
    }

    internal async Task SavePlaceAsync(Guid placeId)
    {
        Response = await Client.PostAsJsonAsync(DefaultStartUrl, new { placeId });
    }

    internal async Task<SavedPlace> SavePlaceSuccessResponseAsync()
    {
        return (await Response.Content.ReadFromJsonAsync<SavedPlace>())!;
    }
}
