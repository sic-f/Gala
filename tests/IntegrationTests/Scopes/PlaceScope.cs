using Core.Dto;
using Core.PlaceAggregate;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class PlaceScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => $"api/Places";

    public async Task SubmitPlaceUnauthenticatedAsync(PlaceDetails details)
    {
        using var content = new MultipartFormDataContent();
        BuildPayload.Compute(details, content);

        Response = await Client.PostAsync(DefaultStartUrl, content);
    }

    public async Task SubmitRequiredDetailsBlankAsync(PlaceDetails details)
    {
        using var content = new MultipartFormDataContent();
        BuildPayload.Compute(details, content);

        Response = await Client.PostAsync(DefaultStartUrl, content);
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

    public async Task UpdatePlaceAsync(Guid placeId, PlaceDetails details, IFormFile? picture = null)
    {
        using var content = new MultipartFormDataContent();
        BuildPayload.Compute(details, content, picture);

        Response = await Client.PutAsync($"{DefaultStartUrl}/{placeId}", content);
    }

    public async Task<IEnumerable<Place>?> GetPlacesAsync(string? offset, int limit)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Place>>($"{DefaultStartUrl}/?offset={offset}&limit={limit}");
    }

    public async Task<PlaceDto> GetPlaceAsync(Guid id)
    {
        return (await Client.GetFromJsonAsync<Core.Dto.PlaceDto>($"{DefaultStartUrl}/{id}"))!;
    }
}