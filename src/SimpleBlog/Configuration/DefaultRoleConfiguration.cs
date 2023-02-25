using System.Diagnostics.CodeAnalysis;

namespace SimpleBlog.Configuration;

public class DefaultRoleConfiguration
{
    public const string SectionName = "DefaultRoles";

    public string? Name { get; set; }
    public string? Permissions { get; set; }

    [MemberNotNullWhen(true, new[] { nameof(Name), nameof(Permissions) })]
    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Name));
    }
}
