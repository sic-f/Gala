using Core.Entities;

namespace Core.Values;

public record Brodiebles
{
    public Guid BroId { get; set; }
    public AccountAvatar? Avatar { get; set; }
}
