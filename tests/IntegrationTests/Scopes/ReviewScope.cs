using Core.Dto;
using Core.PlaceAggregate;
using System.Net.Http.Json;

namespace IntegrationTests.Scopes;

public class ReviewScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Reviews";

    internal async Task SubmitReviewAsync(Review review)
    {
        Response = await Client.PostAsJsonAsync<Review>(DefaultStartUrl, review);
    }

    internal async Task UpdateReviewAsync(Guid id, ReviewDetails details)
    {
        Response = await Client.PutAsJsonAsync($"{DefaultStartUrl}/{id}", details);
    }
}
