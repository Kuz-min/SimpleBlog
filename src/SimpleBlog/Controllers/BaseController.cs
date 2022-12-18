using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using SimpleBlog.Models;
using SimpleBlog.Services;

namespace SimpleBlog.Controllers;

public class BaseController : ControllerBase
{
    private IProfileService? ProfileService
    {
        get
        {
            if (_profileService == null)
                _profileService = HttpContext?.RequestServices?.GetRequiredService<IProfileService>();

            return _profileService;
        }
    }

    protected async Task<Profile?> GetProfileAsync()
    {
        var rawAccountId = User?.Claims?.SingleOrDefault(c => c.Type == OpenIddictConstants.Claims.Subject)?.Value;

        if (string.IsNullOrWhiteSpace(rawAccountId))
            return null;

        if (ProfileService == null)
            return null;

        var accountId = Guid.Parse(rawAccountId);

        return await ProfileService.GetByIdAsync(accountId);
    }

    private IProfileService? _profileService;
}