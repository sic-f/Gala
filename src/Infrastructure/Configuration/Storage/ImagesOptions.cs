namespace Infrastructure.Configuration.Storage
{
    public class ImagesOptions
    {
        public const string Avatar = "Avatar";

        public string IncorrectPath { get; set; } = string.Empty;
        public string CorrectPath { get; set; } = string.Empty;
    }
}