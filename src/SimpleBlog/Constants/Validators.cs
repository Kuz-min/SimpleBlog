using System.Text.RegularExpressions;

namespace SimpleBlog.Constants;

public static class Validators
{
    public static readonly Regex Username = new Regex(@"^[a-z]{1}[a-z0-9|\-|_]{2,23}[a-z0-9]{1}$", RegexOptions.IgnoreCase);
    public static readonly Regex Email = new Regex(@"^[^@\s]+@[^@\s]+$", RegexOptions.IgnoreCase);
}
