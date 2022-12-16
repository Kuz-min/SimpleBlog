using Microsoft.AspNetCore.Authorization;
using SimpleBlog.Models;

namespace SimpleBlog.Authorization;

public class PostSameOwnerAuthorizationHandler : AuthorizationHandler<SameOwnerRequirement, (Post post, Profile profile)>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameOwnerRequirement requirement, (Post post, Profile profile) resource)
    {
        if (resource.post != null && resource.profile != null && resource.post.OwnerId == resource.profile.Id)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
