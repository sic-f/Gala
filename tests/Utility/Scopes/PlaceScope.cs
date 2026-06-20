using System.Net.Http.Json;
using Utility.Dto;

namespace Utility.Scopes;

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
        Response = await Client.PostAsJsonAsync<PlaceDetails>(DefaultStartUrl, details);
    }

    public async Task DeletePlaceAsync(Guid placeId)
    {
        Response = await Client.DeleteAsync($"{DefaultStartUrl}/{placeId}");
    }
}