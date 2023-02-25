namespace SimpleBlog.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime CreatedOn { get; set; }

    public Guid OwnerId { get; set; }
    public Profile Owner { get; set; } = default!;

    public ICollection<Post_PostTag> Tags { get; set; } = default!;
}
