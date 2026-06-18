using System.Net;
using Utility.Scopes;
using Utility.Dto;

namespace IntegrationTests.Place.Submit;

[Collection("Application Factory Collection")]
public class PublicAccess(PlaceScope app) : BaseTest(app, isAuthenticated: false), IClassFixture<PlaceScope>
{
    [Fact]
    public async Task UnauthenticatedOrUnregisteredVisitorTest()
    {
        await WhenBroSubmitsPlace();
        ThenPlaceWillNotBeCreated();
    }

    private async Task WhenBroSubmitsPlace()
    {
        PlaceDetails placeDetails = new(Name: string.Empty, Address: string.Empty);
        await app.SubmitPlaceUnauthenticatedAsync(placeDetails);
    }

    private void ThenPlaceWillNotBeCreated()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, app.ResponseStatusCode);
    }
}