using OpenIddict.Abstractions;

namespace SimpleBlog.StartupTasks;

public class AuthenticationStartupTask
{
    private const string CLIENT_ID = "simple-blog-web-client";

    public AuthenticationStartupTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        using var scope = _serviceProvider.CreateScope();

        var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var app = await appManager.FindByClientIdAsync(CLIENT_ID);

        if (app != null)
            return;

        await appManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = CLIENT_ID,
            ClientSecret = CLIENT_ID,//useless for web client
            DisplayName = CLIENT_ID,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,

                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                OpenIddictConstants.Permissions.ResponseTypes.Token,
            }
        });
    }

    private readonly IServiceProvider _serviceProvider;
}