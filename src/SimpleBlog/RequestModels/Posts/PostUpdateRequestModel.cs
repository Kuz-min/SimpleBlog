using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Utilities;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class PostUpdateRequestModel : IValidatableObject
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public IFormFile? Image { get; set; }
    public IEnumerable<int>? TagIds { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Title))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Title) }));

        if (string.IsNullOrWhiteSpace(Content))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Content) }));

        if (Image != null)
            if (Image.Length == 0 || !Image.IsImage())
                errors.Add(new ValidationResult("INVALID", new[] { nameof(Image) }));

        return errors;
    }
}
