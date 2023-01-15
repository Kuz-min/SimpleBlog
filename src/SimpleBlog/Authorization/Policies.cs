namespace SimpleBlog.Authorization;

public static class Policies
{
    public const string SameOwner = nameof(SameOwner);
    public const string PostTagFullAccess = nameof(PostTagFullAccess);
}
