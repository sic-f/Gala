using Utility.Scopes;
using static Ui.Dto.Place;
using PlaceErrors = Utility.Errors.PlaceErrors;

namespace IntegrationTests.Place;

public class SubmitPlaceTest(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());
    private readonly PlaceErrors _errors = new(app);

    [Fact]
    public async Task RequiredDetailsBlankTestAsync()
    {
        GivenAuthenticatedUser();
        await WhenUserSubmitsWithRequiredDetailsNotPresent();
        ThenAllRequiredFieldsForPlaceWillHaveErrorMessages();
    }

    private static void GivenAuthenticatedUser()
    {
        return; // client already signed in during test setup
    }

    private async Task WhenUserSubmitsWithRequiredDetailsNotPresent()
    {

        PlaceDetails placeDetails = new(Name: string.Empty, Address: string.Empty);
        await app.SubmitRequiredDetailsBlankAsync(placeDetails);
    }

    private void ThenAllRequiredFieldsForPlaceWillHaveErrorMessages()
    {
        Assert.Equal("The Name field is required.", _errors.BlankNameError());
        Assert.Equal("The Address field is required.", _errors.BlankAddressError());
    }
}
