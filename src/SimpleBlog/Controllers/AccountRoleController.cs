using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
using System.Data;

namespace SimpleBlog.Controllers;

[Route("api/account-roles")]
public class AccountRoleController : BaseApiController<AccountRoleController>
{
    public AccountRoleController(IAccountRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    [AllowAnonymous]
    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetByNameAsync([FromRoute] string roleName)
    {
        var role = await _roleService.GetByNameAsync(roleName);

        if (role == null)
            return NotFound();

        return Ok(Map<AccountRoleViewModel>(role));
    }

    [AllowAnonymous]
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
