namespace Core.Exceptions;

public class SaveOwnPlaceException : Exception
{
    public SaveOwnPlaceException() : base() { }
    public SaveOwnPlaceException(string message) : base(message) { }
    public SaveOwnPlaceException(string message, Exception inner) : base(message, inner) { }
}
