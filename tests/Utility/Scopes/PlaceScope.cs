using System.Net.Http.Json;
using static Ui.Dto.Place;

namespace Utility.Scopes;

public class PlaceScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => $"api/Places";

    public async Task SubmitRequiredDetailsBlankAsync(PlaceDetails placeDetails)
    {
        Response = await Client.PostAsJsonAsync<PlaceDetails>(DefaultStartUrl, placeDetails);
    }
}
