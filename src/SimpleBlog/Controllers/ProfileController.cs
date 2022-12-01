using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Models;
using SimpleBlog.Services;
using SimpleBlog.ViewModels.ModelExtensions;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfileController : BaseController
{
    public ProfileController(ILogger<ProfileController> logger, IProfileService profiles)
    {
        _logger = logger;
        _profiles = profiles;
    }

    [HttpGet("{profileId:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int profileId)
    {
        Profile? profile;
        try
        {
            profile = await _profiles.GetByIdAsync(profileId);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (profile == null)
            return NotFound();

        return Ok(profile.ToViewModel());
    }

    private readonly ILogger<ProfileController> _logger;
    private readonly IProfileService _profiles;
}