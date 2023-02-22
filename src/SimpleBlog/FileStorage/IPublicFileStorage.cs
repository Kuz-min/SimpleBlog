namespace SimpleBlog.FileStorage;

public interface IPublicFileStorage
{
    Task<Uri> CreateOrUpdateFileAsync(string type, string name, Stream stream);
}