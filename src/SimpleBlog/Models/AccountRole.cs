using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Models;

public class AccountRole : IdentityRole<Guid>
{
    public ICollection<AccountRoleClaim> Claims { get; set; } = default!;

    public AccountRole()
    {
        Id = Guid.NewGuid();
    }
}
