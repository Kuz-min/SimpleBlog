using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class PostCreateRequestModel : IValidatableObject
{
    //[BindProperty]
    public string Title { get; set; } = string.Empty;

    //[BindProperty]
    public string Content { get; set; } = string.Empty;

    //[BindProperty]
    public IEnumerable<int>? TagIds { get; set; } = default;

    //Validation
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