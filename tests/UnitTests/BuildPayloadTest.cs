using System.Net.Http.Headers;
using Ui.Dto;
using Utility.Helpers;
using Ui.Services;

namespace UnitTests;

public class BuildPayloadTest
{
    [Fact]
    public void BuildPayloadStateTest()
    {

        PlaceDetails details = new()
        {
            Name = "Name detail",
            Address = "Address detail",
            Description = "Description detail",
            Picture = ImageHelper.MockImageFormFile(assembly: "UnitTests"),
        };
        MultipartFormDataContent formData = [];

        BuildPayload.Compute(details, formData);

        Assert.Collection(formData,
            fd =>
            {
                Assert.IsType<StringContent>(fd);
                Assert.Equal(new MediaTypeHeaderValue("text/plain", "utf-8"), fd.Headers.ContentType);
            },
            fd =>
            {
                Assert.IsType<StringContent>(fd);
                Assert.Equal(new MediaTypeHeaderValue("text/plain", "utf-8"), fd.Headers.ContentType);
            },
            fd =>
            {
                Assert.IsType<StringContent>(fd);
                Assert.Equal(new MediaTypeHeaderValue("text/plain", "utf-8"), fd.Headers.ContentType);
            },
            fd =>
            {
                Assert.IsType<StreamContent>(fd);
                Assert.Equal(new MediaTypeHeaderValue("image/jpeg"), fd.Headers.ContentType);
            });
    }
}
