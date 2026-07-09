using System.ComponentModel.DataAnnotations;

namespace Core.Dto;


public record PlacePictureLean(Guid Id, string Small, Guid PlaceId);

public record PlacePicture(Guid Id,
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
