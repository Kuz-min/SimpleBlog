﻿using Microsoft.AspNetCore.Identity;
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

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var configs = new List<DefaultAccountConfiguration>();

        foreach (var rawConfig in configuration.GetSection(DefaultAccountConfiguration.SectionName).GetChildren())
        {
            var config = new DefaultAccountConfiguration();
            rawConfig.Bind(config, options => options.BindNonPublicProperties = true);
            configs.Add(config);
        }

        if (configs.Count == 0)
            return;

        if (!configs.All(config => config.IsValid()))
            throw new InvalidOperationException("Default Account Configuration is not valid");

        var accountManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        foreach (var config in configs)
        {
            var account = await accountManager.FindByNameAsync(config.Name);

            if (account is null)
            {
                account = new Account()
                {
                    UserName = config.Name,
                    Email = config.Email,
                };

                await accountManager.CreateAsync(account, config.Password);

                foreach (var role in config.Roles.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    await accountManager.AddToRoleAsync(account, role);
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