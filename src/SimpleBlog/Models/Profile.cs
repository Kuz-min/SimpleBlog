namespace SimpleBlog.Models;

public class Profile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }

    //public Guid AccountId { get; set; }
    //public Account Account { get; set; }

    public ICollection<Post> Posts { get; set; }
}