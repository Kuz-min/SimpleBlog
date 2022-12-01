using Microsoft.EntityFrameworkCore;
using SimpleBlog.Models;

namespace SimpleBlog.Database;

public interface IBlogDatabase
{
    DbSet<Post> Posts { get; }
    DbSet<Profile> Profiles { get; }
    DbSet<PostTag> PostTags { get; }
    DbSet<Post_PostTag> Post_PostTags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}