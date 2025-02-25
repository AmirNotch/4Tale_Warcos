using System.ComponentModel.DataAnnotations;

namespace Lobby.Validation.Attributes;

public class ValidCountAttribute : ValidationAttribute
{
    public bool IsOptional { get; set; }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (IsOptional && value == null)
        {
            return ValidationResult.Success!;
        }
        if (value == null || value is not int intValue)
        {
            return new ValidationResult("Value is not an integer");
        }
        if (intValue <= 0)
        {
            return new ValidationResult("Count shall be positive");
        }
        if (intValue > 500)
        {
            return new ValidationResult("Value shall not be greater than 500");
        }
        return ValidationResult.Success!;
    }
}
