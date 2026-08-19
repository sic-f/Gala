using Core.Dto;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class SavedReviewScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Saved-Reviews";

    internal async Task<IEnumerable<SavedDto>?> GetSavedReviewsAsync(string offset, int limit)
    {
        return await Client.GetFromJsonAsync<IEnumerable<SavedDto>>(
            $"{DefaultStartUrl}?offset={offset}&limit={limit}");
    }

    internal async Task SavedReviewAsync(Guid saveableId)
    {
        Response = await Client.PostAsJsonAsync($"{DefaultStartUrl}", new { saveableId });
    }
}