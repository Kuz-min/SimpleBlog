using Microsoft.EntityFrameworkCore;
using SimpleBlog.Database;
using SimpleBlog.Models;

namespace SimpleBlog.Services;

public class ProfileService : IProfileService
{
    public ProfileService(ILogger<ProfileService> logger, IBlogDatabase database)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<Profile?> GetByIdAsync(Guid id)
    {
        return await _database.Profiles.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Profile> InsertAsync(Profile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        await _database.Profiles.AddAsync(profile);
        await _database.SaveChangesAsync();

        return profile;
    }

    public async Task UpdateAsync(Profile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        _database.Profiles.Update(profile);
        await _database.SaveChangesAsync();
    }

    private readonly ILogger<ProfileService> _logger;
    private readonly IBlogDatabase _database;
}
