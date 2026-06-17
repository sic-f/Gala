using Utility.Dto;
using Utility.Errors;
using Utility.Scopes;

namespace IntegrationTests.Place.Submit;

[Collection("Application Factory Collection")]
public class RequiredDetailsBlank(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    private readonly PlaceErrors _errors = new(app);
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task RequiredDetailsBlankTestAsync()
    {
        await WhenUserSubmitsWithRequiredDetailsNotPresent();
        ThenAllRequiredFieldsForPlaceWillHaveErrorMessages();
    }

    private async Task WhenUserSubmitsWithRequiredDetailsNotPresent()
    {
        PlaceDetails placeDetails = new(Name: string.Empty, Address: string.Empty);
        await app.SubmitRequiredDetailsBlankAsync(placeDetails);
    }

    private void ThenAllRequiredFieldsForPlaceWillHaveErrorMessages()
    {
        Assert.Equal("Name of place is required", _errors.Name);
        Assert.Equal("Address is required", _errors.Address);
    }
}