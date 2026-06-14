using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset Expires { get; set; }
    public DateTimeOffset? Revoked { get; set; }
    public string? ReplacedByToken { get; set; }
    public DateTimeOffset CreatedAt { get; }
    public Guid BroId { get; set; }

    public bool IsActive => Revoked is null && DateTimeOffset.UtcNow < Expires;
}