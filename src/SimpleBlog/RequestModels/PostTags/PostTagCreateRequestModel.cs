using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class PostTagCreateRequestModel : IValidatableObject
{
    public string Title { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Title))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Title) }));

        return errors;
    }
}
