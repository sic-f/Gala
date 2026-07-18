using System.ComponentModel.DataAnnotations;

namespace Core.Dto;

public record ReviewDetails
{
    [Required]
    public string Content { get; set; } = string.Empty;
    public Guid PlaceId { get; set; }
};
public record ReviewDto(Guid Id, string? Headline, string Content);