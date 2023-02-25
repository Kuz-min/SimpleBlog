namespace SimpleBlog.ViewModels;

public class ProfileViewModel
{
    public string id { get; set; } = default!;
    public string name { get; set; } = default!;
    public DateTime createdOn { get; set; }
    public Uri? image { get; set; }
}
