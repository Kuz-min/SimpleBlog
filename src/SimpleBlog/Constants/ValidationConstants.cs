using System.Text.RegularExpressions;

namespace SimpleBlog.Constants;

public static class ValidationConstants
{
    public static readonly Regex USERNAME = new Regex(@"^[a-z]{1}[a-z0-9|\-|_]{2,23}[a-z0-9]{1}$", RegexOptions.IgnoreCase);
    public static readonly Regex EMAIL = new Regex(@"^[^@\s]+@[^@\s]+$", RegexOptions.IgnoreCase);
}
