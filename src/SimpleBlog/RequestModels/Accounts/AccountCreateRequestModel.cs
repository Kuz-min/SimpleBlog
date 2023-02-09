using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Constants;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class AccountCreateRequestModel : IValidatableObject
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    //Validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Username))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Username) }));
        else if (!ValidationConstants.USERNAME.IsMatch(Username))
            errors.Add(new ValidationResult("INVALID", new[] { nameof(Username) }));

        if (string.IsNullOrWhiteSpace(Email))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Email) }));
        else if (!ValidationConstants.EMAIL.IsMatch(Email))
            errors.Add(new ValidationResult("INVALID", new[] { nameof(Email) }));

        if (string.IsNullOrWhiteSpace(Password))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Password) }));

        return errors;
    }
}