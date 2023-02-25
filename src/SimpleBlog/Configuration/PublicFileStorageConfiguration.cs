namespace SimpleBlog.Configuration;

public class PublicFileStorageConfiguration
{
    public const string SectionName = "PublicFileStorage";

    public string? RootPath { get; set; }
    public Dictionary<string, string>? TypePaths { get; set; }
}
