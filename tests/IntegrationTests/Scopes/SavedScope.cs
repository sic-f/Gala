using Core.Dto;
using System.Net.Http.Json;
using Utility.Dto;

namespace IntegrationTests.Scopes;

public class SavedScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => $"api/Saves";

    internal async Task<IEnumerable<SavedPlaceResponse>> GetAllSavedPlacesAsync(Guid broId)
    {
        return (await Client.GetFromJsonAsync<IEnumerable<SavedPlaceResponse>>($"{DefaultStartUrl}?broId={broId}"))!;
    }

    internal async Task SavePlaceAsync(Guid placeId)
    {
        Response = await Client.PostAsJsonAsync(DefaultStartUrl, new { placeId });
    }

    internal async Task<SavedResponse> SavePlaceSuccessResponseAsync()
    {
        return (await Response.Content.ReadFromJsonAsync<SavedResponse>())!;
    }
}
