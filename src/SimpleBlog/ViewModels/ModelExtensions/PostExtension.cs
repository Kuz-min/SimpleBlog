using SimpleBlog.Models;

namespace SimpleBlog.ViewModels.ModelExtensions;

public static class PostExtension
{
    public static PostViewModel ToViewModel(this Post post) => new PostViewModel()
    {
        id = post.Id,
        title = post.Title,
        content = post.Content,
        createdOn = post.CreatedOn,

        ownerId = post.OwnerId.ToString(),

        tagIds = post.Tags?.Select(tag => tag.PostTagId).ToArray(),
    };
}