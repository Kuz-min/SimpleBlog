using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Database;
using SimpleBlog.Models;

namespace SimpleBlog.Services;

public class AccountRoleService : IAccountRoleService
{
    public AccountRoleService(ILogger<AccountRoleService> logger, IBlogDatabase database, RoleManager<AccountRole> roleManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public async Task<AccountRole?> GetByNameAsync(string name)
    {
        return await _database.Roles.Where(r => r.Name == name).Include(r => r.Claims).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AccountRole>> GetAllAsync()
    {
        return await _database.Roles.Include(r => r.Claims).ToListAsync();
    }

    private readonly ILogger<AccountRoleService> _logger;
    private readonly IBlogDatabase _database;
    private readonly RoleManager<AccountRole> _roleManager;
}