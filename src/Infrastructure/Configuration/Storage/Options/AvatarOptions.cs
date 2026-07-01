using Core.Interfaces;

namespace Infrastructure.Configuration.Storage.Options
{
    public class AvatarOptions : IPathSwapElements
    {
        public const string Avatar = "Avatar";

        public string IncorrectPath { get; set; } = string.Empty;
        public string CorrectPath { get; set; } = string.Empty;
    }
}