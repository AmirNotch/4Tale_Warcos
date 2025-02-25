using System.ComponentModel.DataAnnotations;

namespace Lobby.Validation.Attributes;

public class ValidGuidAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || value is not Guid userId)
        {
            return new ValidationResult("Invalid ID");
        }
        if (userId == Guid.Empty)
        {
            return new ValidationResult("ID shall not be empty");
        }
        return ValidationResult.Success!;
    }
}