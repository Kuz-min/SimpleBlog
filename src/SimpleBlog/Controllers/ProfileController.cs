using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Authorization;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

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

        return Ok(Map<ProfileViewModel>(profile));
    }

    [Authorize]
    [Authorize(Policies.SameOwner)]
    [HttpPut("{profileId:guid}/image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(524_288)]//512 kib
    public async Task<IActionResult> UpdateImageAsync([FromRoute] Guid profileId, [FromForm] IFormFile file)
    {
        var (contentType, contentSubType) = file.ContentType.Split('/') is [var a, var b] ? (a, b) : throw new Exception("invalid content type");

        if (file.Length == 0 || contentType != "image")
            return BadRequest();

        var profile = await _profileService.GetByIdAsync(profileId);

        if (profile == null)
            return BadRequest();

        var name = $"{profileId}.{contentSubType}";
        var path = Path.Combine("wwwroot/images/profiles/", name);

        using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream);

        profile.Image = new Uri($"{Request.Scheme}://{Request.Host}/images/profiles/{name}?t={DateTime.Now.ToString("HHmmss")}");
        await _profileService.UpdateAsync(profile);

        return Ok(Map<ProfileViewModel>(profile));
    }

    private readonly IProfileService _profileService;
}