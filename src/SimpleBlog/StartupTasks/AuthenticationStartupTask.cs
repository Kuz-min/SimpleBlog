using OpenIddict.Abstractions;
using SimpleBlog.Configuration;

namespace SimpleBlog.StartupTasks;

public class AuthenticationStartupTask
{
    public AuthenticationStartupTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        using var scope = _serviceProvider.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var clientAppConfigs = new List<ClientApplicationConfiguration>();

        foreach (var rawClientAppConfig in configuration.GetSection(ClientApplicationConfiguration.SectionName).GetChildren())
        {
            var clientAppConfig = new ClientApplicationConfiguration();
            rawClientAppConfig.Bind(clientAppConfig, options => options.BindNonPublicProperties = true);
            clientAppConfigs.Add(clientAppConfig);
        }

        if (!clientAppConfigs.All(config => config.IsValid()))
            throw new InvalidOperationException("Client Application Configuration is not valid");

        var clientAppManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var clientAppConfig in clientAppConfigs)
        {
            var clientApp = await clientAppManager.FindByClientIdAsync(clientAppConfig.Id);

            if (clientApp is null)
            {
                var clientAppDescriptor = new OpenIddictApplicationDescriptor()
                {
                    ClientId = clientAppConfig.Id,
                    ClientSecret = clientAppConfig.Secret,
                    DisplayName = clientAppConfig.Name,
                };
                foreach (var permission in clientAppConfig.Permissions.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
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