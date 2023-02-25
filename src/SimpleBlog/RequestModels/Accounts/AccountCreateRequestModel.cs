using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Constants;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class AccountCreateRequestModel : IValidatableObject
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Username))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Username) }));
        else if (!Validators.Username.IsMatch(Username))
            errors.Add(new ValidationResult("INVALID", new[] { nameof(Username) }));

        if (string.IsNullOrWhiteSpace(Email))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Email) }));
        else if (!Validators.Email.IsMatch(Email))
            errors.Add(new ValidationResult("INVALID", new[] { nameof(Email) }));

        if (string.IsNullOrWhiteSpace(Password))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Password) }));

        return errors;
    }
}
