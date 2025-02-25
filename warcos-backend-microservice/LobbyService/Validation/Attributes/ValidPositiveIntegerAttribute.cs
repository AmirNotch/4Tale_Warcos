using System.ComponentModel.DataAnnotations;

namespace Lobby.Validation.Attributes;

public class ValidPositiveIntegerAttribute : ValidationAttribute
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
            return new ValidationResult("Value shall be positive");
        }
        return ValidationResult.Success!;
    }
}
