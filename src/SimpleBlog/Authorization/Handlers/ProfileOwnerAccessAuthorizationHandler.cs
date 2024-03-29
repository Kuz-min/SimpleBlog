using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;

namespace SimpleBlog.Authorization;

public class ProfileOwnerAccessAuthorizationHandler : AuthorizationHandler<OwnerAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerAccessRequirement requirement)
    {
        var rawAccountId = context.User.Claims.SingleOrDefault(c => c.Type == OpenIddictConstants.Claims.Subject)?.Value;

        var httpContext = context.Resource as DefaultHttpContext;

        if (rawAccountId != null && httpContext != null && rawAccountId == httpContext.Request.RouteValues["profileId"]?.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
