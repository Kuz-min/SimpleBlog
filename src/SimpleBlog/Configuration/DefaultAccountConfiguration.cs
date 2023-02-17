namespace SimpleBlog.Configuration;

public class DefaultAccountConfiguration
{
    public static string SectionName = "DefaultAccounts";

    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;

    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email));
    }
}
