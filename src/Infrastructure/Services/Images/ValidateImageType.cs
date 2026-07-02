using Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Images;

public class ValidateImageType
{
    public string[] ValidTypes { get; } = ["image/jpeg", "image/jpg", "image/png"];

    public void Validate(IFormFile image)
    {
        Compute(image);
    }

    public void Validate(Values.Images images)
    {
        if (images.Picture is not null)
        {
            Compute(images.Picture);
        }

        if (images.Pictures is not null)
        {
            foreach (IFormFile picture in images.Pictures)
            {
                Compute(picture);
            }
        }
    }

    private void Compute(IFormFile formFile)
    {
        if (!ValidTypes.Contains(formFile.ContentType))
        {
            throw new ImageTypeException($"\"{formFile.FileName}\" not of jpeg, jpg, png");
        }
    }
}