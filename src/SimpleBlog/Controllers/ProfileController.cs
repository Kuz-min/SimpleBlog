using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Models;
using SimpleBlog.Services;
using SimpleBlog.ViewModels.ModelExtensions;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfileController : BaseController
{
    public ProfileController(ILogger<ProfileController> logger, IProfileService profileService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
    }

    [HttpGet("{profileId:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid profileId)
    {
        Profile? profile;
        try
        {
            profile = await _profileService.GetByIdAsync(profileId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (profile == null)
            return NotFound();

        return Ok(profile.ToViewModel());
    }

    private readonly ILogger<ProfileController> _logger;
    private readonly IProfileService _profileService;
}