using Core.Dto;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services.Pictures;

public class ResizePersistPicture(string fileName,
                                  Stream sourceStream,
                                  ImageDimensions imageDimensions,
                                  ResizeMode resizeMode)
{
    public async Task ComputeAsync()
    {
        InitializeDirectory(fileName);

        using var fileStream = new FileStream(fileName, FileMode.Create);
        var resizeOptions = new ResizeOptions
        {
            Size = new Size(imageDimensions.Width, imageDimensions.Height),
            Mode = resizeMode
        };

        using var image = await Image.LoadAsync(sourceStream);
        sourceStream.Seek(0, SeekOrigin.Begin);

        image.Mutate(x => x.Resize(resizeOptions));

        await image.SaveAsJpegAsync(fileStream);
    }

    private static void InitializeDirectory(string fileName)
    {
        if (!Directory.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);
        }
    }
}
