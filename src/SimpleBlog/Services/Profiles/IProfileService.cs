using SimpleBlog.Models;

namespace SimpleBlog.Services;

public interface IProfileService
{
    Task<Profile?> GetByIdAsync(int id);
    Task<Profile?> GetByAccountIdAsync(string accountId);
    Task<Profile> InsertAsync(Profile profile);
    Task UpdateAsync(Profile profile);
}