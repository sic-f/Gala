namespace Core.Values;

public record ImageDimensions(int Width, int Height);
public record PicturePaths
{
    public string Normal { get; set; } = string.Empty;
    public string Small { get; set; } = string.Empty;
    public string NormalLocal { get; set; } = string.Empty;
    public string SmallLocal { get; set; } = string.Empty;
    public string? ExistingNormal { get; set; }
    public string? ExistingSmall { get; set; }
}
public record PictureUpload(Stream Stream, PicturePaths Paths, string ContentType, string FileName);
public record PictureUploads(PictureUpload? Picture, IEnumerable<PictureUpload>? Pictures);
