using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lobby.Validation.Attributes;

public class ValidJsonAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (!(value is string stringValue))
        {
            return new ValidationResult("Invalid type, expected a JSON string");
        }

        try
        {
            JsonConvert.DeserializeObject(stringValue);
            return ValidationResult.Success;
        }
        catch (JsonException)
        {
            return new ValidationResult("Invalid JSON format");
        }
    }
}