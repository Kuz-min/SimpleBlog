using SimpleBlog.Models;

namespace SimpleBlog.Services;

public interface IPostService
{
    Task<Post?> GetByIdAsync(int id);
    Task<IEnumerable<Post>> GetByIdAsync(IEnumerable<int> ids);
    Task<IEnumerable<Post>> SearchAsync(IEnumerable<int>? tagIds, int offset, int count);
    Task<Post> InsertAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
}