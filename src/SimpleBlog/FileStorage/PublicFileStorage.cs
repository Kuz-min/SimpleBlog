using Microsoft.Extensions.Options;

namespace SimpleBlog.FileStorage;

public class PublicFileStorage : IPublicFileStorage
{
    private const string DEFAULT_ROOT_PATH = "wwwroot";

    public PublicFileStorage(IOptions<PublicFileStorageConfiguration> configuration)
    {
        var rootPath = TrimPath(configuration.Value.RootPath);
        _rootPath = !string.IsNullOrEmpty(rootPath) ? rootPath : DEFAULT_ROOT_PATH;

        _typePaths = configuration.Value.TypePaths.Select(path => (Key: path.Key, Value: TrimPath(path.Value)))
            .ToDictionary(p => p.Key, p => p.Value);
    }

    public async Task<Uri> CreateOrUpdateFileAsync(string type, string name, Stream stream)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException($"null or empty {nameof(type)}");

        if (string.IsNullOrEmpty(name))
            throw new ArgumentException($"null or empty {nameof(name)}");

        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var typePath = _typePaths.GetValueOrDefault(type) ?? string.Empty;

        var filePath = Path.Combine(_rootPath, typePath, name);

        using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream);

        var url = string.IsNullOrEmpty(typePath) ? string.Empty : $"/{typePath}";
        return new Uri($"{url}/{name}?t={DateTime.Now.ToString("HHmmss")}", UriKind.Relative);
    }

    private string TrimPath(string path) => path.Trim(new[] { ' ', '/' });

    private readonly string _rootPath;
    private readonly Dictionary<string, string> _typePaths;
}