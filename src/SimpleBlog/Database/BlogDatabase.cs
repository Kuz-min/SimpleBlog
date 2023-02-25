using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Models;
using System.Reflection;

namespace SimpleBlog.Database;

public class BlogDatabase : IdentityDbContext<Account, AccountRole, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, AccountRoleClaim, IdentityUserToken<Guid>>, IBlogDatabase
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<PostTag> PostTags { get; set; }

    public BlogDatabase(DbContextOptions<BlogDatabase> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
