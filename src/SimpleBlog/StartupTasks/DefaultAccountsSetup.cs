using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SimpleBlog.Configuration;
using SimpleBlog.Models;
using SimpleBlog.Services;

namespace SimpleBlog.StartupTasks;

public class DefaultAccountsSetup
{
    public DefaultAccountsSetup(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        using var scope = _serviceProvider.CreateScope();

        var configs = _serviceProvider.GetRequiredService<IOptions<List<DefaultAccountConfiguration>>>()?.Value;

        if (configs == null || configs.Count == 0)
            return;

        var accountManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        foreach (var config in configs)
        {
            if (!config.IsValid())
                continue;

            var account = await accountManager.FindByNameAsync(config.Name);

            if (account == null)
            {
                account = new Account()
                {
                    UserName = config.Name,
                    Email = config.Email,
                };

                await accountManager.CreateAsync(account, config.Password);

                if (!string.IsNullOrEmpty(config.Roles))
                {
                    foreach (var role in config.Roles.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                    {
                        await accountManager.AddToRoleAsync(account, role);
                    }
                }

                var profile = new Profile()
                {
                    Id = account.Id,
                    Name = account.UserName,
                    CreatedOn = DateTime.Now,
                };

                await profileService.InsertAsync(profile);
            }
        }
    }

    private readonly IServiceProvider _serviceProvider;
}
