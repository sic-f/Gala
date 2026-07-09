namespace Core.Interfaces;

public interface IPathSwapElements
{
    string IncorrectPath { get; }
    string CorrectPath { get; }
    public string ImageKindSegmentWithWindowsDirectorySeparator { get; set; }
    public string ImageKindSegmentWitGeneralDirectorySeparator { get; set; }
}
