using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using SimpleBlog.Configuration;

namespace SimpleBlog.StartupTasks;

public class DefaultClientApplicationsSetup
{
    public DefaultClientApplicationsSetup(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        using var scope = _serviceProvider.CreateScope();

        var configs = _serviceProvider.GetRequiredService<IOptions<List<DefaultClientAppConfiguration>>>()?.Value;

        if (configs == null || configs.Count == 0)
            return;

        var clientAppManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var config in configs)
        {
            if (!config.IsValid())
                continue;

            var clientApp = await clientAppManager.FindByClientIdAsync(config.Id);

            if (clientApp == null)
            {
                var clientAppDescriptor = new OpenIddictApplicationDescriptor()
                {
                    ClientId = config.Id,
                    ClientSecret = config.Secret,
                    DisplayName = config.Name,
                };

                foreach (var permission in config.Permissions.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                    clientAppDescriptor.Permissions.Add(permission);

                await clientAppManager.CreateAsync(clientAppDescriptor);
            }
        };

        //OpenIddictConstants.Permissions.Endpoints.Token,
        //OpenIddictConstants.Permissions.GrantTypes.Password,
        //OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
        //OpenIddictConstants.Permissions.ResponseTypes.Token,
    }

    private readonly IServiceProvider _serviceProvider;
}
