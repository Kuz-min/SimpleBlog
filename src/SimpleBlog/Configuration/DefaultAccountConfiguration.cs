namespace SimpleBlog.Configuration;

public class DefaultAccountConfiguration
{
    public static string SectionName = "DefaultAccounts";

    public string Name { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Roles { get; private set; } = string.Empty;

    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email));
    }
}
