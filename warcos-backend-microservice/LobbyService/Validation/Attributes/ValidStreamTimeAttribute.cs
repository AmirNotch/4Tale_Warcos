using System.ComponentModel.DataAnnotations;

namespace Lobby.Validation.Attributes;

public class ValidStreamTimeAttribute : ValidationAttribute
{
    public bool ValidateEndTime { get; set; }
    public override bool IsValid(object? value)
    {
        if (value == null || value is not DateTimeOffset dateTime)
        {
            return false;
        }
        
        // Если проверяем EndTime, просто возвращаем true (основная проверка будет в IsValid с контекстом)
        if (ValidateEndTime)
        {
            return true;
        }

        // Проверка StartTime на то, что оно не в прошлом
        if (dateTime < DateTimeOffset.UtcNow)
        {
            return false;
        }

        return true;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Value cannot be null.");
        }

        if (value is not DateTimeOffset offset)
        {
            return new ValidationResult("Invalid datetime format.");
        }
        
        if (!IsValid(offset))
        {
            if (!ValidateEndTime)
            {
                return new ValidationResult("StartTime must be in the present or future.");
            }
            return new ValidationResult("EndTime  must be in the present or future.");
        }

        if (ValidateEndTime)
        {
            // Проверка EndTime на соответствие StartTime
            var instance = validationContext.ObjectInstance;
            var startTimeProperty = instance.GetType().GetProperty("StartTime");
            var startTimeValue = startTimeProperty?.GetValue(instance) as DateTimeOffset?;

            if (startTimeValue.HasValue && offset < startTimeValue.Value)
            {
                return new ValidationResult("EndTime must be after StartTime.");
            }
        }

        return ValidationResult.Success;
    }
}