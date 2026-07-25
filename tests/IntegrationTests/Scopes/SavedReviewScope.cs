namespace IntegrationTests.Scopes;

public class SavedReviewScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/SavedReviews";

    internal async Task SavedReviewAsync(Guid id)
    {
        Response = await Client.PostAsync($"{DefaultStartUrl}/{id}", null);
    }
}