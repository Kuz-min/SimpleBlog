namespace SimpleBlog.Configuration;

public class DefaultRoleConfiguration
{
    public static string SectionName = "DefaultRoles";

    public string Name { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;

    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Name));
    }
}