namespace Core.Dto;

public class Response
{
    public record TokenableResponse(string Token, string Message);
}
