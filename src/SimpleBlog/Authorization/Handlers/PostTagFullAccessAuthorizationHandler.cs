using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using SimpleBlog.Services;

namespace SimpleBlog.Authorization;

public class PostTagFullAccessAuthorizationHandler : AuthorizationHandler<PostTagFullAccessRequirement>
{
    public PostTagFullAccessAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PostTagFullAccessRequirement requirement)
    {
        using var scope = _serviceProvider.CreateScope();

        var accountRoleService = scope.ServiceProvider.GetRequiredService<IAccountRoleService>();

        var userRoles = context.User?.Claims?.Where(c => c.Type == OpenIddictConstants.Claims.Role).Select(c => c.Value);

        if (userRoles == null || userRoles.Count() < 1)
            return;

        foreach (var roleName in userRoles)
        {
            var role = await accountRoleService.GetByNameAsync(roleName);

            if (role == null)
                continue;

            if (role.Claims.Any(claim => claim.ClaimType == SimpleBlogClaims.Permission && claim.ClaimValue == Policies.PostTagFullAccess))
                context.Succeed(requirement);
        }
    }

    private readonly IServiceProvider _serviceProvider;
}