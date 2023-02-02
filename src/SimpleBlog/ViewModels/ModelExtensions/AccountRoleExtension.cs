using SimpleBlog.Authorization;
using SimpleBlog.Models;

namespace SimpleBlog.ViewModels.ModelExtensions;

public static class AccountRoleExtension
{
    public static AccountRoleViewModel ToViewModel(this AccountRole role) => new AccountRoleViewModel()
    {
        name = role.Name,
        permissions = role?.Claims.Where(c => c.ClaimType == SimpleBlogClaims.Permission && !string.IsNullOrEmpty(c.ClaimValue)).Select(c => c.ClaimValue!).ToArray(),
    };
}