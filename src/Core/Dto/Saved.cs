namespace Core.Dto;

public record Saved(Guid SaveableId);
public record SavedDto
{
    public Guid SaveableId { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public string Kind { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
};
