using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using SimpleBlog.Models;

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
        var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AccountRole>>();

        var userRoles = context.User?.Claims?.Where(c => c.Type == OpenIddictConstants.Claims.Role).Select(c => c.Value).ToList();

        if (userRoles == null || userRoles.Count == 0)
            return;

        foreach (var roleName in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                continue;

            var claims = await _roleManager.GetClaimsAsync(role);

            if (claims == null || claims.Count == 0)
                continue;

            if (claims.Any(claim => claim.Type == SimpleBlogClaims.Permission && claim.Value == Policies.PostTagFullAccess))
                context.Succeed(requirement);
        }
    }

    private readonly IServiceProvider _serviceProvider;
}