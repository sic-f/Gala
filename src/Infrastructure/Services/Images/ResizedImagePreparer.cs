using Core.Values;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services.Images;

public class ResizedImagePreparer(IFormFile formFile,
                                  ImageDimensions imageDimensions,
                                  ResizeMode resizeMode) : ImagePreparer(formFile)
{
    public override async Task PrepareAsync(FileStream stream)
    {
        var resizeOptions = new ResizeOptions
        {
            Size = new Size(imageDimensions.Width, imageDimensions.Height),
            Mode = resizeMode
        };

        var image = await Image.LoadAsync(_formFile.OpenReadStream());

        image.Mutate(x => x.Resize(resizeOptions));
        await image.SaveAsJpegAsync(stream);

        stream.Position = 0;
    }
}