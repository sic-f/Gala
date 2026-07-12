using Core.PlaceAggregate;
using IntegrationTests.Services;
using System.Net.Http.Json;
using Utility.Dto;

namespace IntegrationTests.Scopes;

public class PlaceScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => $"api/Places";

    public async Task SubmitPlaceUnauthenticatedAsync(PlaceDetails placeDetails)
    {
        Response = await Client.PostAsJsonAsync(DefaultStartUrl, placeDetails);
    }

    public async Task SubmitRequiredDetailsBlankAsync(PlaceDetails placeDetails)
    {
        Response = await Client.PostAsJsonAsync<PlaceDetails>(DefaultStartUrl, placeDetails);
    }

    public async Task SubmitPlaceWithLongInvalidValuesAsync(PlaceDetails details)
    {
        using var content = new MultipartFormDataContent();
        BuildPayload.Compute(details, content);

        Response = await Client.PostAsync(DefaultStartUrl, content);
    }

    public async Task DeletePlaceAsync(Guid placeId)
    {
        Response = await Client.DeleteAsync($"{DefaultStartUrl}/{placeId}");
    }

    public async Task UpdatePlaceAsync(Guid placeId, PlaceDetails details)
    {
        using var content = new MultipartFormDataContent();
        BuildPayload.Compute(details, content);

        Response = await Client.PutAsync($"{DefaultStartUrl}/{placeId}", content);
    }

    public async Task<IEnumerable<Place>?> GetPlacesAsync(string? lastModifiedDate, int fetchLimit)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Place>>($"{DefaultStartUrl}/?offset={lastModifiedDate}&limit={fetchLimit}");
    }
}