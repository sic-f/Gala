using System.ComponentModel.DataAnnotations;

namespace Core.Dto;

public record CommentDetails
{
    [Required]
    [MaxLength(8000)]
    public string Body { get; set; } = string.Empty;
}
public record CommentDto(
    Guid Id,
    string Body,
    string CommenterName,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastModified,
    string? CommenterAvatar = null);