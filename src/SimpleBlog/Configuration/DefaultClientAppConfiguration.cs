using System.Diagnostics.CodeAnalysis;

namespace SimpleBlog.Configuration;

public class DefaultClientAppConfiguration
{
    public const string SectionName = "DefaultClientApplications";

    public string? Id { get; set; }
    public string? Secret { get; set; } 
    public string? Name { get; set; }
    public string? Permissions { get; set; }

    [MemberNotNullWhen(true, new[] { nameof(Id), nameof(Secret), nameof(Name), nameof(Permissions) })]
    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Secret) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Permissions));
    }
}
