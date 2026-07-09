using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Services.Images;

public class DeleteImage(ApplicationDbContext context)
{
    public async Task ComputeAsync(IImageDeletable picture, GetImagesPath getImagesPath)
    {
        string normalFileName = new Uri(picture.Normal).Segments.Last();
        string smallFileName = new Uri(picture.Small).Segments.Last();

        File.Delete(getImagesPath.FileNamePath(normalFileName));
        File.Delete(getImagesPath.FileNamePath(smallFileName));

        context.Remove(picture);
    }
}
