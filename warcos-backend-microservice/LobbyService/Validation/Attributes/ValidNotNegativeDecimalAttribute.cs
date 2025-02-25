using System.ComponentModel.DataAnnotations;

namespace Lobby.Validation.Attributes;

public class ValidNotNegativeDecimalAttribute : ValidationAttribute
{
    public bool IsOptional { get; set; }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (IsOptional && value == null)
        {
            return ValidationResult.Success!;
        }
        if (value == null || value is not decimal intValue)
        {
            return new ValidationResult("Value is not an decimal");
        }
        if (intValue < 0)
        {
            return new ValidationResult("Value shall not be negative");
        }
        return ValidationResult.Success!;
    }
}