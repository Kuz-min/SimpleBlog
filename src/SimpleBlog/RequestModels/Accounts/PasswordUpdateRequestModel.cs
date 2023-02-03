using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class PasswordUpdateRequestModel : IValidatableObject
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(CurrentPassword))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(CurrentPassword) }));

        if (string.IsNullOrWhiteSpace(NewPassword))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(NewPassword) }));

        return errors;
    }
}