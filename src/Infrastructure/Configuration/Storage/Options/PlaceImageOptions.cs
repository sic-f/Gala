using Core.Interfaces;

namespace Infrastructure.Configuration.Storage.Options;

public class PlaceImageOptions : IPathSwapElements
{
    public const string Place = "Place";

    public string IncorrectPath { get; set; } = string.Empty;
    public string CorrectPath { get; set; } = string.Empty;
}
