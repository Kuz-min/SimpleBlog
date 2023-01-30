using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Authorization;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;
using SimpleBlog.ViewModels.ModelExtensions;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/account-roles")]
public class AccountRoleController : ControllerBase
{
    public AccountRoleController(ILogger<AccountRoleController> logger, RoleManager<AccountRole> roleManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetByNameAsync([FromRoute] string roleName)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);

            var permissions = claims.Where(c => c.Type == SimpleBlogClaims.Permission).Select(c => c.Value);

            return Ok(role.ToViewModel(permissions));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var roles = await _roleManager.Roles.ToArrayAsync();

            if (roles == null)
                return NotFound();

            var vm = new List<AccountRoleViewModel>(roles.Length);

            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                var permissions = claims.Where(c => c.Type == SimpleBlogClaims.Permission).Select(c => c.Value);
                vm.Add(role.ToViewModel(permissions));
            }

            return Ok(vm);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private readonly ILogger<AccountRoleController> _logger;
    private readonly RoleManager<AccountRole> _roleManager;
}