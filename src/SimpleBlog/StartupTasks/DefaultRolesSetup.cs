using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SimpleBlog.Authorization;
using SimpleBlog.Configuration;
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

        if (!configs.All(config => config.IsValid()))
            throw new InvalidOperationException("Default Role Configuration is not valid");

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AccountRole>>();

        foreach (var config in configs)
        {
            var role = await roleManager.FindByNameAsync(config.Name);

            if (role is null)
            {
                role = new AccountRole();
                role.Name = config.Name;
                await roleManager.CreateAsync(role);

                foreach (var permission in config.Permissions.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    await roleManager.AddClaimAsync(role, new Claim(SimpleBlogClaims.Permission, permission));
                }
            }
        }
    }

    private readonly IServiceProvider _serviceProvider;
}