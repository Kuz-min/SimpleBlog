namespace SimpleBlog.Models;

public class Post_PostTag
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; } = default!;

    public int PostTagId { get; set; }
    public PostTag PostTag { get; set; } = default!;
}
