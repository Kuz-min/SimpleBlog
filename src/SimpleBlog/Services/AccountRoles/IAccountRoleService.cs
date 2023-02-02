using SimpleBlog.Models;

namespace SimpleBlog.Services;

public interface IAccountRoleService
{
    Task<AccountRole?> GetByNameAsync(string name);
    Task<IEnumerable<AccountRole>> GetAllAsync();
}