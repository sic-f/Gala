namespace Core.Exceptions;

public class BlankPictureCaptionException : Exception
{
    public BlankPictureCaptionException() : base() { }
    public BlankPictureCaptionException(string message) : base(message) { }
    public BlankPictureCaptionException(string message, Exception inner) : base(message, inner) { }
}
