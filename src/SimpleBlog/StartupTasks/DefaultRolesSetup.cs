using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SimpleBlog.Configuration;
using SimpleBlog.Constants;
using SimpleBlog.Models;
using System.Security.Claims;

namespace SimpleBlog.StartupTasks;

public class DefaultRolesSetup
{
    public DefaultRolesSetup(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        using var scope = _serviceProvider.CreateScope();

        var configs = _serviceProvider.GetRequiredService<IOptions<List<DefaultRoleConfiguration>>>()?.Value;

        if (configs == null || configs.Count == 0)
            return;

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AccountRole>>();

        foreach (var config in configs)
        {
            if (!config.IsValid())
                continue;

            var role = await roleManager.FindByNameAsync(config.Name);

            if (role == null)
            {
                role = new AccountRole();
                role.Name = config.Name;
                await roleManager.CreateAsync(role);

                foreach (var permission in config.Permissions.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    await roleManager.AddClaimAsync(role, new Claim(Claims.Permission, permission));
                }
            }
        }
    }

    private readonly IServiceProvider _serviceProvider;
}
