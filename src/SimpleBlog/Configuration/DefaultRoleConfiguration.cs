namespace SimpleBlog.Configuration;

public class DefaultRoleConfiguration
{
    public static string SectionName = "DefaultRoles";

    public string Name { get; private set; } = string.Empty;
    public string Permissions { get; private set; } = string.Empty;

    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Name));
    }
}