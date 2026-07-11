namespace Core.Exceptions;

public class UnauthorizedContributionAccessException : Exception
{
    public UnauthorizedContributionAccessException() : base() { }
    public UnauthorizedContributionAccessException(string message) : base(message) { }
    public UnauthorizedContributionAccessException(string message, Exception inner) : base(message, inner) { }
}
