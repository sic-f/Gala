namespace Core.Dto;

public record Saved(Guid SaveableId);
public record SavedDto(Guid SaveableId, string Headline, string Picture, DateTimeOffset CreatedAt);
