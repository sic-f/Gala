using Core.Values;

namespace Core.Interfaces;

public interface IGetImagePaths
{
    PicturePaths GetURIs<T>(UrlSegments urlSegments, PicturePaths paths) where T : IImageTypeSegmentable;
    PicturePaths GetPicturePaths<T>(PicturePaths paths) where T : IImageTypeSegmentable;
    void ReInitializeFileName();
}
