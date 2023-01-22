using SimpleBlog.Models;

namespace SimpleBlog.ViewModels.ModelExtensions;

public static class AccountRoleExtension
{
    public static AccountRoleViewModel ToViewModel(this AccountRole role, IEnumerable<string>? permissions) => new AccountRoleViewModel()
    {
        name = role.Name,
        permissions = permissions?.ToArray(),
    };
}