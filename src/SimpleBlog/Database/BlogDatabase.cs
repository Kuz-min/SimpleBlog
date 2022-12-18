using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Database.Configurations;
using SimpleBlog.Models;

namespace SimpleBlog.Database;

public class BlogDatabase : IdentityDbContext<Account, AccountRole, Guid>, IBlogDatabase
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<Post_PostTag> Post_PostTags { get; set; }

    public BlogDatabase(DbContextOptions<BlogDatabase> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PostConfiguration());
        modelBuilder.ApplyConfiguration(new PostTagConfiguration());
        modelBuilder.ApplyConfiguration(new Post_PostTagConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileConfiguration());
    }
}