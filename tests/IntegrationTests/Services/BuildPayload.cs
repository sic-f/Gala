using Microsoft.AspNetCore.Http;
using System.Reflection;
using Utility.Dto;

namespace IntegrationTests.Services;

public class BuildPayload
{
    public static void Compute(PlaceDetails details, MultipartFormDataContent content)
    {
        foreach (PropertyInfo detail in details.GetType().GetProperties())
        {
            string? value = detail.GetValue(details, null)?.ToString();
            if (value is null || detail.GetValue(details, null) is IFormFile) continue;

            content.Add(new StringContent(value), detail.Name);
        }

        if (details.Picture is not null)
        {
            var pictureField = new StreamContent(details.Picture.OpenReadStream());
            pictureField.Headers.Add("Content-Type", details.Picture.ContentType);
            content.Add(pictureField, "picture", details.Picture.FileName);
        }
    }
}
