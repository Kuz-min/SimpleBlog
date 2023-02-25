using System.Diagnostics.CodeAnalysis;

namespace SimpleBlog.Configuration;

public class DefaultAccountConfiguration
{
    public const string SectionName = "DefaultAccounts";

    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Roles { get; set; }

    [MemberNotNullWhen(true, new[] { nameof(Name), nameof(Password), nameof(Email) })]
    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email));
    }
}
