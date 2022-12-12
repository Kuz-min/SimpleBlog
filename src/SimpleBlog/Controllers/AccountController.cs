using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Models;
using SimpleBlog.RequestModels;
using SimpleBlog.Services;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController : ControllerBase
{
    public AccountController(ILogger<AccountController> logger, UserManager<Account> accountManager, IProfileService profiles)
    {
        _logger = logger;
        _accountManager = accountManager;
        _profiles = profiles;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] AccountCreateRequestModel request)
    {
        Account? account;
        try
        {
            account = await _accountManager.FindByNameAsync(request.Username);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (account != null)
            return StatusCode(StatusCodes.Status409Conflict);

        account = new Account
        {
            UserName = request.Username,
            Email = request.Email,
        };

        IdentityResult? result;
        try
        {
            result = await _accountManager.CreateAsync(account, request.Password);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError);

        _logger.LogInformation($"Account created with id {account.Id}");

        Profile? profile;
        try
        {
            profile = await _profiles.InsertAsync(new Profile()
            {
                Name = request.Username,
                AccountId = account.Id,
                CreatedOn = DateTime.Now,
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        _logger.LogInformation($"User created with id {profile.Id}");

        return StatusCode(StatusCodes.Status201Created);
    }

    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<Account> _accountManager;
    private readonly IProfileService _profiles;
}