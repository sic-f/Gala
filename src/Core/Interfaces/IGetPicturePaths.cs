using Core.Values;

namespace Core.Interfaces;

public interface IGetPicturePaths
{
    PicturePaths GetURIs<T>(UrlSegments urlSegments, PicturePaths paths) where T : IImageTypeSegmentable;
    PicturePaths GetImagePaths<T>(PicturePaths paths) where T : IImageTypeSegmentable;
    void ReInitializeFileName();
}
