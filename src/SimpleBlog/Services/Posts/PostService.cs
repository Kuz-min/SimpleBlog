using Microsoft.EntityFrameworkCore;
using SimpleBlog.Database;
using SimpleBlog.Models;

namespace SimpleBlog.Services;

public class PostService : IPostService
{
    public PostService(ILogger<PostService> logger, IBlogDatabase database)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _database.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetByIdAsync(IEnumerable<int> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        return await _database.Posts.Include(p => p.Tags).Where(p => ids.Contains(p.Id)).ToListAsync();
    }

    public async Task<IEnumerable<Post>> SearchAsync(IEnumerable<int>? tagIds, int offset, int count)
    {
        IQueryable<Post> request = _database.Posts.AsNoTracking();

        request = request.Include(p => p.Tags);

        if (tagIds != null && tagIds.Count() > 0)
            request = request.Where(post => post.Tags.Count(tag => tagIds.Contains(tag.PostTagId)) == tagIds.Count());

        request = request.OrderBy(q => q.CreatedOn);

        if (offset > 0)
            request = request.Skip(offset);

        request = request.Take(count);

        _logger.LogDebug(request.ToQueryString());

        return await request.ToListAsync();
    }

    public async Task<Post> InsertAsync(Post post)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        await _database.Posts.AddAsync(post);
        await _database.SaveChangesAsync();

        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        _database.Posts.Update(post);
        await _database.SaveChangesAsync();
    }

    public async Task DeleteAsync(Post post)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        _database.Posts.Remove(post);
        await _database.SaveChangesAsync();
    }

    private readonly ILogger<PostService> _logger;
    private readonly IBlogDatabase _database;
}
