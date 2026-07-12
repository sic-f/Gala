using Core.Values;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services.Images;

public class ResizedImagePreparer(Stream pictureStream,
                                  ImageDimensions imageDimensions,
                                  ResizeMode resizeMode) : ImagePreparer(pictureStream)
{
    public override async Task PrepareAsync(FileStream stream)
    {
        var resizeOptions = new ResizeOptions
        {
            Size = new Size(imageDimensions.Width, imageDimensions.Height),
            Mode = resizeMode
        };

        using var image = await Image.LoadAsync(_pictureStream);
        _pictureStream.Seek(0, SeekOrigin.Begin);

        image.Mutate(x => x.Resize(resizeOptions));
        await image.SaveAsJpegAsync(stream);
    }
}