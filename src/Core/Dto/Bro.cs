namespace Core.Dto;

public record LoginCredentials(string Email, string Password);
public record BroWithFullName(string Email, string Password, string? FirstName = null, string? LastName = null);
