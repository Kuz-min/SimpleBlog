using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Models;

public class AccountRoleClaim : IdentityRoleClaim<Guid>
{
    public AccountRole Role { get; set; } = default!;
}
