namespace SimpleBlog.ViewModels;

public class PostViewModel
{
    public int id { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public DateTime createdOn { get; set; }

    public string ownerId { get; set; }

    public int[]? tagIds { get; set; }
}