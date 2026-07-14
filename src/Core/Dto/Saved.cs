namespace Core.Dto;

public record SavedPlaceResponse(Guid SaveableId, Guid SaverId);
public record Saved(Guid PlaceId);
