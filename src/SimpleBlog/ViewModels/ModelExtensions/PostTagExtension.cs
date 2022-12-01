using SimpleBlog.Models;

namespace SimpleBlog.ViewModels.ModelExtensions;

public static class PostTagExtension
{
    public static PostTagViewModel ToViewModel(this PostTag tag) => new PostTagViewModel()
    {
        id = tag.Id,
        title = tag.Title,
    };
}