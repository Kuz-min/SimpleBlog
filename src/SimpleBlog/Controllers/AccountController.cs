using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Models;
using SimpleBlog.RequestModels;
using SimpleBlog.Services;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController : BaseController<AccountController>
{
    public AccountController(UserManager<Account> accountManager, IProfileService profileService)
    {
        _accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] AccountCreateRequestModel request)
    {
        var account = await _accountManager.FindByNameAsync(request.Username);

        if (account != null)
            return Conflict();

        account = new Account
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var result = await _accountManager.CreateAsync(account, request.Password);

        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError);

        Logger.LogInformation($"Account with id {account.Id} created");

        var profile = await _profileService.InsertAsync(new Profile()
        {
            Id = account.Id,
            Name = request.Username,
            CreatedOn = DateTime.Now,
        });

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        Logger.LogInformation($"Profile with id {profile.Id} created");

        return StatusCode(StatusCodes.Status201Created);
    }

    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> UpdatePasswordAsync([FromBody] PasswordUpdateRequestModel request)
    {
        var id = GetAccountId();

        var account = await _accountManager.FindByIdAsync(id.ToString());

        if (account == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        var result = await _accountManager.ChangePasswordAsync(account, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            return BadRequest();

        return Ok();
    }

    private readonly UserManager<Account> _accountManager;
    private readonly IProfileService _profileService;
}