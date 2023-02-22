namespace SimpleBlog.FileStorage;

public class PublicFileStorageConfiguration
{
    public const string SectionName = "PublicFileStorage";
    public string RootPath { get; set; } = string.Empty;
    public Dictionary<string, string> TypePaths { get; set; } = new();
}