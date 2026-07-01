namespace Infrastructure.Configuration.Storage.Options
{
    public class AvatarOptions
    {
        public const string Avatar = "Avatar";

        public string IncorrectPath { get; set; } = string.Empty;
        public string CorrectPath { get; set; } = string.Empty;
    }
}