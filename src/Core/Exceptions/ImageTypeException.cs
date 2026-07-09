namespace Core.Exceptions
{
    [Serializable]
    public class ImageTypeException : Exception
    {
        public ImageTypeException() : base() { }
        public ImageTypeException(string message) : base(message) { }
        public ImageTypeException(string message, Exception inner) : base(message, inner) { }
    }
}
