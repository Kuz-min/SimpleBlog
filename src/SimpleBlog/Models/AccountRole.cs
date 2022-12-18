using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Models;

public class AccountRole : IdentityRole<Guid>
{
    public AccountRole()
    {
        Id = Guid.NewGuid();
    }
}