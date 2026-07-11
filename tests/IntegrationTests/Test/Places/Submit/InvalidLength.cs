using IntegrationTests.Errors;
using IntegrationTests.Scopes;
using Utility.Dto;
using Utility.Helpers;

namespace IntegrationTests.Test.Places.Submit;

[Collection("Application Factory Collection")]
public class InvalidLength(PlaceScope app) : BaseTest(app), IClassFixture<PlaceScope>
{
    private readonly PlaceErrors _errors = new(app);
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task LongValuesTestAsync()
    {
        await WhenBroSubmitsPlaceWithInvalidLongValuesAsync();
        ThenPlaceWillHaveErrorsAndNotSubmitted();
    }

    private async Task WhenBroSubmitsPlaceWithInvalidLongValuesAsync()
    {
        var invalidValues = new LongInvalidPlaceValues();
        PlaceDetails details = new()
        {
            Name = invalidValues.Name,
            Description = invalidValues.Description,
            Information = invalidValues.Information,
            Address = invalidValues.Address,
            Mobile = invalidValues.Mobile,
            Landline = invalidValues.Landline,
            Email = invalidValues.Email,
            Website = invalidValues.Website,
            Facebook = invalidValues.Facebook,
            Instagram = invalidValues.Instagram
        };

        await app.SubmitPlaceWithLongInvalidValuesAsync(details);
    }

    private void ThenPlaceWillHaveErrorsAndNotSubmitted()
    {
        Assert.Equal("Exceeds 75 characters", _errors.Name);
        Assert.Equal("Exceeds 155 characters", _errors.Description);
        Assert.Equal("Exceeds 50,000 characters", _errors.Information);
        Assert.Equal("Exceeds 95 characters", _errors.Address);
        Assert.Equal("Exceeds 15 digits", _errors.Mobile);
        Assert.Equal("Exceeds 15 digits", _errors.Landline);
        Assert.Equal("Exceeds 254 characters", _errors.Email);
        Assert.Equal("Exceeds 2,083 characters", _errors.Website);
        Assert.Equal("Exceeds 2,083 characters", _errors.Facebook);
        Assert.Equal("Exceeds 30 characters", _errors.Instagram);
    }
}