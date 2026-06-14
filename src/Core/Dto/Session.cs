namespace Core.Dto;

public class Session
{
    public record RefreshTokenables(string RefreshToken, Guid BroId);
    public record RefreshTokenResponse(string RefreshToken, string Token);
}
