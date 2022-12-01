using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using SimpleBlog.Models;
using SimpleBlog.Services;

namespace SimpleBlog.Controllers;

public class BaseController : ControllerBase
{
    private IProfileService? Profiles
    {
        get
        {
            if (_profiles == null)
                _profiles = HttpContext?.RequestServices?.GetRequiredService<IProfileService>();

            return _profiles;
        }
    }

    protected async Task<Profile?> GetProfileAsync()
    {
        var accountId = User?.Claims?.SingleOrDefault(c => c.Type == OpenIddictConstants.Claims.Subject)?.Value;

        if (string.IsNullOrWhiteSpace(accountId))
            return null;

        if (Profiles == null)
            return null;

        return await Profiles.GetByAccountIdAsync(accountId);
    }

    private IProfileService? _profiles;
}