using SimpleBlog.Models;

namespace SimpleBlog.Services;

public interface IPostTagService
{
    Task<PostTag?> GetByIdAsync(int id);
    Task<IEnumerable<PostTag>> GetByIdAsync(IEnumerable<int> ids);
    Task<IEnumerable<PostTag>> GetAllAsync();
    Task<PostTag> InsertAsync(PostTag tag);
    Task UpdateAsync(PostTag tag);
    Task DeleteAsync(PostTag tag);
}
