using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Models;

public class Account : IdentityUser<Guid>
{
    //public Profile Profile { get; set; }

    public Account()
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
    }
}