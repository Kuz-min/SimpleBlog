using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class PostUpdateRequestModel : IValidatableObject
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public IEnumerable<int>? TagIds { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Title))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Title) }));

        if (string.IsNullOrWhiteSpace(Content))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Content) }));

        return errors;
    }
}
