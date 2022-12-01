using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class AccountCreateRequestModel : IValidatableObject
{
    //[BindProperty]
    public string Username { get; set; } = string.Empty;

    //[BindProperty]
    //[EmailAddress]
    public string Email { get; set; } = string.Empty;

    //[BindProperty]
    public string Password { get; set; } = string.Empty;

    //Validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();


        if (string.IsNullOrWhiteSpace(Username))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Username) }));
        else if (!Regex.IsMatch(Username, @"^[a-z]+[a-z0-9|\-|_]*[a-z0-9]$", RegexOptions.IgnoreCase))
            errors.Add(new ValidationResult("NOT_VALIDE", new[] { nameof(Username) }));


        if (string.IsNullOrWhiteSpace(Email))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Email) }));
        else if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+$", RegexOptions.IgnoreCase))
            errors.Add(new ValidationResult("NOT_VALIDE", new[] { nameof(Email) }));


        if (string.IsNullOrWhiteSpace(Password))
            errors.Add(new ValidationResult("EMPTY", new[] { nameof(Password) }));

        return errors;
    }
}