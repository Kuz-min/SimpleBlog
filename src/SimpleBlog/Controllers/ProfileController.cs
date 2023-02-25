using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Constants;
using SimpleBlog.FileStorage;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers;

[Route("api/profiles")]
public class ProfileController : BaseApiController<ProfileController>
{
    public ProfileController(IPublicFileStorage fileStorage, IProfileService profileService)
    {
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
    }

    [AllowAnonymous]
    [HttpGet("{profileId:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid profileId)
    {
        var profile = await _profileService.GetByIdAsync(profileId);

        if (profile == null)
            return NotFound();

        return Ok(Map<ProfileViewModel>(profile));
    }

    [Authorize]
    [Authorize(Policies.OwnerAccess)]
    [HttpPut("{profileId:guid}/image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(524_288)]//512 kib
    public async Task<IActionResult> UpdateImageAsync([FromRoute] Guid profileId, [FromForm] IFormFile file)
    {
        var (contentType, contentSubType) = file.ContentType.Split('/') is [var a, var b] ? (a, b) : (null, null);

        if (file.Length == 0 || contentType != "image")
            return BadRequest();

        var profile = await _profileService.GetByIdAsync(profileId);

        if (profile == null)
            return BadRequest();

        var name = $"{profileId}.{contentSubType}";
        using var stream = file.OpenReadStream();
        var uri = await _fileStorage.CreateOrUpdateFileAsync(FileTypes.ProfileImage, name, stream);

        profile.Image = uri;
        await _profileService.UpdateAsync(profile);

        return Ok(Map<ProfileViewModel>(profile));
    }

    private readonly IPublicFileStorage _fileStorage;
    private readonly IProfileService _profileService;
}
