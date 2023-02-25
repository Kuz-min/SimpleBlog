using SimpleBlog.Models;

namespace SimpleBlog.Services;

public interface IProfileService
{
    Task<Profile?> GetByIdAsync(Guid id);
    Task<Profile> InsertAsync(Profile profile);
    Task UpdateAsync(Profile profile);
}
