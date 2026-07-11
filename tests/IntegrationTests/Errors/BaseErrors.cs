using System.Text.Json;

namespace IntegrationTests.Errors;

public class BaseErrors(ApplicationFactory<Program> app)
{
    private JsonElement? _errors;
    private readonly ResponseErrors _responseErrors = new(app);

    public JsonElement? Errors
    {
        get { return _errors ??= _responseErrors.ErrorsResponseBody().Result; }
        set { _errors = value; }
    }
}