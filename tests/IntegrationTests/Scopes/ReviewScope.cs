using Core.Dto;
using Core.PlaceAggregate;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class ReviewScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Reviews";

    internal async Task DeleteReviewAsync(Guid id)
    {
        Response = await Client.DeleteAsync($"{DefaultStartUrl}/{id}");
    }

    internal async Task FetchPaginatedReviewsAsync(int offset, int limit)
    {
        Response = await Client.GetAsync($"{DefaultStartUrl}?offset={offset}&limit={limit}");
    }

    internal async Task<IEnumerable<ReviewDto>> FetchPaginatedReviewsSuccessResponseAsync()
    {
        return (await Response.Content.ReadFromJsonAsync<IEnumerable<ReviewDto>>())!;
    }

    internal async Task SubmitReviewAsync(Review review)
    {
        Response = await Client.PostAsJsonAsync<Review>(DefaultStartUrl, review);
    }

    internal async Task UpdateReviewAsync(Guid id, ReviewDetails details)
    {
        Response = await Client.PutAsJsonAsync($"{DefaultStartUrl}/{id}", details);
    }
}
