using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using SimpleBlog.ViewModels.ModelExtensions;
using System.Data;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/account-roles")]
public class AccountRoleController : ControllerBase
{
    public AccountRoleController(ILogger<AccountRoleController> logger, IAccountRoleService roleService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetByNameAsync([FromRoute] string roleName)
    {
        try
        {
            var role = await _roleService.GetByNameAsync(roleName);

            if (role == null)
                return NotFound();

            return Ok(role.ToViewModel());
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
            var roles = await _roleService.GetAllAsync();

            if (roles == null || roles.Count() < 1)
                return NotFound();

            var vm = roles.Select(r => r.ToViewModel());

            return Ok(vm);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private readonly ILogger<AccountRoleController> _logger;
    private readonly IAccountRoleService _roleService;
}