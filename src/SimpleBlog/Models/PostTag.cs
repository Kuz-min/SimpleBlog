namespace SimpleBlog.Models;

public class PostTag
{
    public int Id { get; set; }
    public string Title { get; set; }
    public ICollection<Post_PostTag> Posts { get; set; }
}