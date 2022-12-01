using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class ProfileUpdateRequestModel : IValidatableObject
{
    public string Name { get; set; } = string.Empty;

    //Validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Name) }));

        return errors;
    }
}