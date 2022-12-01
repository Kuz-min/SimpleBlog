using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Models;

public class Account : IdentityUser
{
    public Profile Profile { get; set; }
}