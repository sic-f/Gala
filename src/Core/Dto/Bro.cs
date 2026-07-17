using Core.Entities;

namespace Core.Dto;

public record LoginCredentials(string Email, string Password);
public record BroWithFullName(string Email, string Password, string? FirstName = null, string? LastName = null);
public record Brodiebles
{
    public Guid BroId { get; set; }
    public AccountAvatar? Avatar { get; set; }
}