namespace IntegrationTests.Errors;

internal class ReviewErrors(ApplicationFactory<Program> app) : BaseErrors(app)
{
    public string? Content => Errors?.GetProperty(nameof(Content))[0].GetString();
}
