namespace SimpleBlog.ViewModels;

public class ProfileViewModel
{
    public string id { get; set; }
    public string name { get; set; }
    public DateTime createdOn { get; set; }
    public Uri? image { get; set; }
}