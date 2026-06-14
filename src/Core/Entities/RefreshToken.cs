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
    public bool IsTerminated => HasBeenRevoked && IsExpired;
    private bool HasBeenRevoked => Revoked is not null;
    private bool IsExpired => DateTimeOffset.UtcNow > Expires;
}