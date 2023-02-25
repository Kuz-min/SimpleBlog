namespace SimpleBlog.ViewModels;

public class PostViewModel
{
    public int id { get; set; }
    public string title { get; set; } = default!;
    public string content { get; set; } = default!;
    public DateTime createdOn { get; set; }

    public string ownerId { get; set; } = default!;

    public int[]? tagIds { get; set; }
}
