using Azure;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Database;
using SimpleBlog.Models;

namespace SimpleBlog.Services;

public class PostTagService : IPostTagService
{
    public PostTagService(ILogger<PostService> logger, IBlogDatabase database)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<PostTag?> GetByIdAsync(int id)
    {
        return await _database.PostTags.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PostTag>> GetByIdAsync(IEnumerable<int> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        return await _database.PostTags.Where(pt => ids.Contains(pt.Id)).ToListAsync();
    }

    public async Task<IEnumerable<PostTag>> GetAllAsync()
    {
        return await _database.PostTags.ToListAsync();
    }

    public async Task<PostTag> InsertAsync(PostTag tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        await _database.PostTags.AddAsync(tag);
        await _database.SaveChangesAsync();

        return tag;
    }

    public async Task UpdateAsync(PostTag tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        _database.PostTags.Update(tag);
        await _database.SaveChangesAsync();
    }

    public async Task DeleteAsync(PostTag tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        _database.PostTags.Remove(tag);
        await _database.SaveChangesAsync();
    }

    private readonly ILogger<PostService> _logger;
    private readonly IBlogDatabase _database;
}