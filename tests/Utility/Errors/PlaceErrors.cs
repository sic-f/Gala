using Utility.Scopes;

namespace Utility.Errors;

public class PlaceErrors(PlaceScope app) : BaseErrors(app)
{
    public string BlankNameError()
    {
        return Errors?.GetProperty("Name").GetRawText()!;
    }

    public string BlankAddressError()
    {
        return Errors?.GetProperty("Address").GetRawText()!;
    }
}
