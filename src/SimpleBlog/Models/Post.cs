namespace SimpleBlog.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedOn { get; set; }

    public int OwnerId { get; set; }
    public Profile Owner { get; set; }

    public ICollection<Post_PostTag> Tags { get; set; }
}