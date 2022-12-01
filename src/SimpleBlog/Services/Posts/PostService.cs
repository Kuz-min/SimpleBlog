using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Database;
using SimpleBlog.Database.Configurations;
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
        return await _database.Posts.Include(p => p.Tags).Where(p => ids.Contains(p.Id)).ToListAsync();
    }

    public async Task<IEnumerable<Post>> SearchAsync(IEnumerable<int>? tagIds, int offset, int count)
    {
        IQueryable<Post> request;

        if (tagIds != null && tagIds.Count() > 0)
        {
            //request = request.Where(post => tagIds.All(tagId => post.Tags.Any(t => t.PostTagId == tagId))); dont work

            var sqlTagIds = ToSqlParameters(tagIds, 0);
            var sqlRaw = $"""
                SELECT p.Id, p.Title, p.Content, p.CreatedOn, p.OwnerId
                FROM [{PostConfiguration.TableName}] AS p
                INNER JOIN [{Post_PostTagConfiguration.TableName}] AS ppt ON p.Id = ppt.PostId
                WHERE ppt.PostTagId IN ({string.Join(',', sqlTagIds)})
                GROUP BY p.Id, p.Title, p.Content, p.CreatedOn, p.OwnerId 
                HAVING COUNT(p.Id) = {sqlTagIds.Count()}
                """;

            request = _database.Posts.FromSqlRaw(sqlRaw, sqlTagIds.ToArray());
        }
        else
        {
            request = _database.Posts.AsQueryable();
        }

        //request = request.Include(p => p.Owner);
        request = request.Include(p => p.Tags);

        request = request.OrderBy(q => q.CreatedOn);

        if (offset > 0)
            request = request.Skip(offset);

        request = request.Take(count);

        //_logger.LogInformation(request.ToQueryString());

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

    private IEnumerable<SqlParameter> ToSqlParameters<T>(IEnumerable<T> array, int startIndex)
    {
        var sqlParameters = new List<SqlParameter>();
        var index = startIndex;

        foreach (var item in array)
            sqlParameters.Add(new SqlParameter($"@p{index++}", item));

        return sqlParameters;
    }

    private readonly ILogger<PostService> _logger;
    private readonly IBlogDatabase _database;
}