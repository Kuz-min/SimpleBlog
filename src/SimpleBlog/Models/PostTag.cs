namespace SimpleBlog.Models;

public class PostTag
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public ICollection<Post_PostTag> Posts { get; set; } = default!;
}
