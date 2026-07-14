using Core.Dto;
using Core.Interfaces;

namespace Core.Services.Pictures;

public class GetPicturePaths : IGetPicturePaths
{
    private string _fileName = default!;
    private string _smallFileName = default!;

    public GetPicturePaths()
    {
        GenerateRandomFileNames();
    }

    public PicturePaths GetURIs<T>(UrlSegments urlSegments, PicturePaths paths) where T : IImageTypeSegmentable
    {
        string normal = $"{urlSegments.Scheme}://{urlSegments.Host}/{ToUrlFriendly(T.ImageTypeSegment)}/{_fileName}";
        string small = $"{urlSegments.Scheme}://{urlSegments.Host}/{ToUrlFriendly(T.ImageTypeSegment)}/{_smallFileName}";

        paths.Normal = normal;
        paths.Small = small;

        return paths;
    }

    public PicturePaths GetImagePaths<T>(PicturePaths paths) where T : IImageTypeSegmentable
    {
        paths.NormalLocal = FileNamePath<T>(_fileName);
        paths.SmallLocal = FileNamePath<T>(_smallFileName);

        return paths;
    }

    public void ReInitializeFileName()
    {
        GenerateRandomFileNames();
    }

    /// <summary>
    /// Returns the absolute path for the image file. 🛣️🚶‍
    /// </summary>
    /// <param name="fileName">The name of the image file. 🗒️</param>
    private static string FileNamePath<T>(string fileName) where T : IImageTypeSegmentable
    {
        string contentRoot = Directory.GetCurrentDirectory();
        string imageTypeSegment = T.ImageTypeSegment;

        return Path.Combine(contentRoot, "wwwroot", imageTypeSegment, fileName);
    }

    private void GenerateRandomFileNames()
    {
        var baseFileName = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();
        _fileName = $"{baseFileName}.jpg";
        _smallFileName = $"{baseFileName}-small.jpg";
    }

    private static string ToUrlFriendly(string pathSegment)
    {
        return pathSegment.ToLower().Replace("\\", "/");
    }
}
