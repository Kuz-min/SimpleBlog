using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using SimpleBlog.Constants;
using SimpleBlog.Services;

namespace SimpleBlog.Authorization;

public class PostFullAccessAuthorizationHandler : AuthorizationHandler<PostFullAccessRequirement>
{
    public PostFullAccessAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PostFullAccessRequirement requirement)
    {
        using var scope = _serviceProvider.CreateScope();

        var accountRoleService = scope.ServiceProvider.GetRequiredService<IAccountRoleService>();

        var userRoles = context.User.Claims.Where(c => c.Type == OpenIddictConstants.Claims.Role).Select(c => c.Value);

        if (userRoles == null || userRoles.Count() == 0)
            return;

        foreach (var roleName in userRoles)
        {
            var role = await accountRoleService.GetByNameAsync(roleName);

            if (role == null)
                continue;

            if (role.Claims.Any(claim => claim.ClaimType == Claims.Permission && claim.ClaimValue == Policies.PostFullAccess))
                context.Succeed(requirement);
        }
    }

    private readonly IServiceProvider _serviceProvider;
}
