namespace SimpleBlog.Configuration;

public class DefaultClientAppConfiguration
{
    public static string SectionName = "DefaultClientApplications";

    public string Id { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;

    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Secret) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Permissions));
    }
}