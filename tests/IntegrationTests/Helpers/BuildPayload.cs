using Core.Dto;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace IntegrationTests.Helpers;

public class BuildPayload
{
    public static void Compute(PlaceDetails details, MultipartFormDataContent content, IFormFile? picture = null)
    {
        foreach (PropertyInfo detail in details.GetType().GetProperties())
        {
            string? value = detail.GetValue(details, null)?.ToString();
            if (value is null || detail.GetValue(details, null) is IFormFile) continue;

            content.Add(new StringContent(value), detail.Name);
        }

        if (picture is not null)
        {
            var pictureField = new StreamContent(picture.OpenReadStream());
            pictureField.Headers.Add("Content-Type", picture.ContentType);
            content.Add(pictureField, "picture", picture.FileName);
        }
    }
}
