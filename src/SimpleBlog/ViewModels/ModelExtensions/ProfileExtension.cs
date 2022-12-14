using SimpleBlog.Models;

namespace SimpleBlog.ViewModels.ModelExtensions;

public static class ProfileExtension
{
    public static ProfileViewModel ToViewModel(this Profile profile) => new ProfileViewModel()
    {
        id = profile.Id.ToString(),
        name = profile.Name,
        createdOn = profile.CreatedOn,
    };
}