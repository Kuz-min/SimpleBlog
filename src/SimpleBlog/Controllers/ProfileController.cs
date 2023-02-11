using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using SimpleBlog.ViewModels.ModelExtensions;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfileController : BaseController<ProfileController>
{
    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
    }

    [HttpGet("{profileId:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid profileId)
    {
        var profile = await _profileService.GetByIdAsync(profileId);

        if (profile == null)
            return NotFound();

        return Ok(profile.ToViewModel());
    }

    private readonly IProfileService _profileService;
}