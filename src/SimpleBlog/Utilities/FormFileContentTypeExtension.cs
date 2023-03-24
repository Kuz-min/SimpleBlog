using SimpleBlog.Constants;

namespace SimpleBlog.Utilities;

public static class FormFileContentTypeExtension
{
    public static bool IsImage(this IFormFile file) => Validators.ImageContentType.IsMatch(file.ContentType);
}
