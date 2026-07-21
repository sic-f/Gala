namespace Core.Exceptions;

public class UnauthorizedReviewModificationException : Exception
{
    public UnauthorizedReviewModificationException() : base() { }
    public UnauthorizedReviewModificationException(string message) : base(message) { }
    public UnauthorizedReviewModificationException(string message, Exception inner) : base(message, inner) { }
}
