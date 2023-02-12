using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
using System.Data;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/account-roles")]
public class AccountRoleController : BaseController<AccountRoleController>
{
    public AccountRoleController(IAccountRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetByNameAsync([FromRoute] string roleName)
    {
        var role = await _roleService.GetByNameAsync(roleName);

        if (role == null)
            return NotFound();

        return Ok(Map<AccountRoleViewModel>(role));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var roles = await _roleService.GetAllAsync();

        if (roles == null || roles.Count() == 0)
            return NotFound();

        var vm = roles.Select(Map<AccountRoleViewModel>);

        return Ok(vm);
    }

    private readonly IAccountRoleService _roleService;
}