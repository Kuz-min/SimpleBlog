namespace SimpleBlog.Models;

public class Profile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedOn { get; set; }
    public Uri? Image { get; set; }
    public ICollection<Post> Posts { get; set; } = default!;
}
