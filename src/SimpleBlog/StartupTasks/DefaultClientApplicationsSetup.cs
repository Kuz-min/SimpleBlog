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

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var configs = new List<DefaultClientAppConfiguration>();

        foreach (var rawConfig in configuration.GetSection(DefaultClientAppConfiguration.SectionName).GetChildren())
        {
            var config = new DefaultClientAppConfiguration();
            rawConfig.Bind(config, options => options.BindNonPublicProperties = true);
            configs.Add(config);
        }

        if (configs.Count == 0)
            return;

        if (!configs.All(config => config.IsValid()))
            throw new InvalidOperationException("Client Application Configuration is not valid");

        var clientAppManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var config in configs)
        {
            var clientApp = await clientAppManager.FindByClientIdAsync(config.Id);

            if (clientApp is null)
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