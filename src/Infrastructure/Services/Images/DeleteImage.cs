using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Services.Images;

public class DeleteImage(ApplicationDbContext context)
{
    /// <summary>
    /// Deletes physical image file on disk.🗑️🖼️
    /// </summary>
    /// <param name="picture">The picture database record to delete the image file with.🗄️</param>
    /// <param name="getImagesPath">The configuration object that contains path information for deleting pictures.🛣️</param>
    /// <returns>void</returns>
    public async Task ComputeAsync(IImageDeletable picture, GetImagesPath getImagesPath)
    {
        string normalFileName = new Uri(picture.Normal).Segments.Last();
        string smallFileName = new Uri(picture.Small).Segments.Last();

        File.Delete(getImagesPath.FileNamePath(normalFileName));
        File.Delete(getImagesPath.FileNamePath(smallFileName));

        context.Remove(picture);
    }

    /// <summary>
    /// Deletes a collection of pictures (IImageDeletable).🗑️
    /// </summary>
    /// <param name="pictures">A collection of pictures (IEnumerable<IImageDeletable>) to be deleted.🖼️📸</param>
    /// <param name="getImagesPath">The configuration object that contains path information for deleting pictures.🛣️</param>
    /// <returns>void</returns>
    public async Task DeleteAllAsync(IEnumerable<IImageDeletable> pictures, GetImagesPath getImagesPath)
    {
        foreach (IImageDeletable picture in pictures)
        {
            await ComputeAsync(picture, getImagesPath);
        }
    }
}
