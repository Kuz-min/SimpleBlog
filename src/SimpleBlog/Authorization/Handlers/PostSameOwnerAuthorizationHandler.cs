using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using SimpleBlog.Models;

namespace SimpleBlog.Authorization;

public class PostSameOwnerAuthorizationHandler : AuthorizationHandler<SameOwnerRequirement, Post>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameOwnerRequirement requirement, Post resource)
    {
        var rawAccountId = context.User?.Claims?.SingleOrDefault(c => c.Type == OpenIddictConstants.Claims.Subject)?.Value;

        var accountId = string.IsNullOrWhiteSpace(rawAccountId) ? Guid.Empty : Guid.Parse(rawAccountId);

        if (resource != null && accountId != Guid.Empty && resource.OwnerId == accountId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}