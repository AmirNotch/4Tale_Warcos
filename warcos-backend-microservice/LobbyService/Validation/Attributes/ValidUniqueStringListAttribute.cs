using System.ComponentModel.DataAnnotations;

namespace Lobby.Validation.Attributes;

public class ValidUniqueStringListAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || value is not List<string> stringList)
        {
            return new ValidationResult("Invalid input list.");
        }

        if (stringList.Count == 0)
        {
            return new ValidationResult("List of Ids cannot be empty.");
        }
        
        var uniqueStrings = new HashSet<string>();
        foreach (var str in stringList)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new ValidationResult("List contains empty or whitespace strings.");
            }

            if (!uniqueStrings.Add(str))
            {
                return new ValidationResult($"Duplicate string found: {str}");
            }
        }

        return ValidationResult.Success!;
    }
}