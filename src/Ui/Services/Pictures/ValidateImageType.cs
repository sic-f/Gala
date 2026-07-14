using Core.Exceptions;

namespace Ui.Services.Pictures;

public class ValidateImageType
{
    public string[] ValidTypes { get; } = ["image/jpeg", "image/jpg", "image/png", "image/webp"];

    public void Validate(IFormFile image)
    {
        Compute(image);
    }

    private void Compute(IFormFile formFile)
    {
        if (formFile is null)
        {
            ArgumentNullException.ThrowIfNull(formFile, nameof(formFile));
        }

        if (!ValidTypes.Contains(formFile.ContentType))
        {
            throw new ImageTypeException($"\"{formFile.FileName}\" not of jpeg, jpg, png");
        }
    }
}