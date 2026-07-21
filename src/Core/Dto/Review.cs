using System.ComponentModel.DataAnnotations;

namespace Core.Dto;

public record ReviewDetails
{
    [MaxLength(75, ErrorMessage= "Must not exceed 75 characters.")]
    public string? Headline { get; set; }
    [Required(ErrorMessage = "Must write a review or cancel.")]
    [MaxLength(50000, ErrorMessage = "Must not exceed 50,000 characters")]
    public string Content { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
};
public record ReviewDto(
    Guid Id,
    string? Headline,
    string Content,
    Guid PlaceId,
    Guid ReviewerId,
    DateTimeOffset CreatedAt);