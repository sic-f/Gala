using System.ComponentModel.DataAnnotations;

namespace Core.Dto;

public record PlacePictureLean(Guid Id, string Small, Guid PlaceId);
public record PlacePictureDto(Guid Id,
                              string Normal,
                              Guid PlaceId,
                              Guid? UploaderId,
                              int Index,
                              string? Caption = null);
public record PictureCaption
{
    [MaxLength(155, ErrorMessage = "Can't exceed more than 155 characters.")]
    public string? Caption { get; set; }
}
public record ModifyPlacePictureCaption(Guid Id, string? Caption, Guid? UploaderId, string Message);
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