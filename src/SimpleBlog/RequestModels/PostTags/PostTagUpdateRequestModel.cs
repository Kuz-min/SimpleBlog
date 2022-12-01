using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class PostTagUpdateRequestModel : IValidatableObject
{
    //[BindProperty]
    public string Title { get; set; } = string.Empty;

    //Validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Title))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Title) }));

        return errors;
    }
}