namespace Core.Dto;

public class Session
{
    public record RefreshTokenable(string Token);
    public record RefreshTokenResponse(string RefreshToken, string Token);
}
