namespace Utility.Dto;

public record SavedPlace(Guid SaveableId, Guid SaverId);
public record SavedPlaceResponse(Guid SaverId);
