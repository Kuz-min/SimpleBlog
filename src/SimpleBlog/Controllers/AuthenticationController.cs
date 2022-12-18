using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SimpleBlog.Models;
using System.Security.Claims;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        UserManager<Account> accountManager,
        SignInManager<Account> signInManager
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    [AllowAnonymous]
    [HttpPost("token"), Produces("application/json")]
    public async Task<IActionResult> TokenAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request != null && request.IsPasswordGrantType())
        {
            var account = await _accountManager.FindByNameAsync(request.Username);
            if (account == null)
            {
                var properties = BuildErrorMessage(OpenIddictConstants.Errors.InvalidGrant, "The username/password couple is invalid.");
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(account, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                var properties = BuildErrorMessage(OpenIddictConstants.Errors.InvalidGrant, "The username/password couple is invalid.");
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var principal = BuildClaimsPrincipal(account);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else if (request != null && request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                var properties = BuildErrorMessage(OpenIddictConstants.Errors.InvalidGrant, "The refresh token is no longer valid.");
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var account = await _accountManager.GetUserAsync(result.Principal);

            if (account == null)
            {
                var properties = BuildErrorMessage(OpenIddictConstants.Errors.InvalidGrant, "The refresh token is no longer valid.");
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var canSignIn = await _signInManager.CanSignInAsync(account);

            if (!canSignIn)
            {
                var properties = BuildErrorMessage(OpenIddictConstants.Errors.InvalidGrant, "The user is no longer allowed to sign in.");
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var principal = BuildClaimsPrincipal(account);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else
        {
            var properties = BuildErrorMessage(OpenIddictConstants.Errors.UnsupportedGrantType, "GrantType is unsupported.");
            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    private AuthenticationProperties BuildErrorMessage(string error, string description)
    {
        return new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description,
        });
    }

    private ClaimsPrincipal BuildClaimsPrincipal(Account account)
    {
        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        identity.AddClaim(ClaimTypes.NameIdentifier, account.Id.ToString());
        identity.AddClaim(OpenIddictConstants.Claims.Subject, account.Id.ToString());

        identity.AddClaim(ClaimTypes.Name, account.UserName);
        identity.AddClaim(OpenIddictConstants.Claims.Name, account.UserName);

        identity.AddClaim(ClaimTypes.Email, account.Email);
        identity.AddClaim(OpenIddictConstants.Claims.Email, account.Email);

        //identity.AddClaim("profile_id", account.Profile.Id.ToString(), OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(new string[] { OpenIddictConstants.Scopes.OfflineAccess });

        return principal;
    }

    private readonly ILogger<AuthenticationController> _logger;
    private readonly UserManager<Account> _accountManager;
    private readonly SignInManager<Account> _signInManager;
}