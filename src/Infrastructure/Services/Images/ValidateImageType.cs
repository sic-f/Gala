using Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Images;

public class ValidateImageType
{
    public string[] ValidTypes { get; } = ["image/jpeg", "image/jpg", "image/png"];

    public void Compute(IFormFile formFile)
    {
        if (!ValidTypes.Contains(formFile.ContentType))
        {
            throw new ImageTypeException($"\"{formFile.FileName}\" not of jpeg, jpg, png");
        }
    }
}