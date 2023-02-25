using Microsoft.AspNetCore.Mvc;
using SimpleBlog.ModelBinders;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.RequestModels;

[BindProperties]
public class PostSearchRequestModel : IValidatableObject
{
    [BindProperty(BinderType = typeof(SeparatedStringToArrayBinder), Name = "tag_ids")]
    public IEnumerable<int>? TagIds { get; set; }
    public int Offset { get; set; } = 0;
    public int Count { get; set; } = 50;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        return errors;
    }
}
