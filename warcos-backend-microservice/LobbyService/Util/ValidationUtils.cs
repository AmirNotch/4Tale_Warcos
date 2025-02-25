using Lobby.Models.WsMessage;
using Lobby.Validation;
using System.ComponentModel.DataAnnotations;

using static Lobby.Validation.ErrorCode;

namespace Lobby.Util;

public class ValidationUtils
{
    public static bool BasicWsValidation(IInputMessageData? inputMessageData, IValidationStorage validationStorage)
    {
        if (inputMessageData == null)
        {
            AddEmptyDataError(validationStorage);
            return false;
        }
        var context = new ValidationContext(inputMessageData, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(inputMessageData, context, validationResults, true))
        {
            validationStorage.AddError(InvalidData, "Request data is invalid");
        }
        return validationStorage.IsValid;
    }

    public static void AddEmptyDataError(IValidationStorage validationStorage)
    {
        validationStorage.AddError(EmptyData, "Request data is empty");
    }

    public static void AddUnknownUserError(IValidationStorage validationStorage, Guid userId)
    {
        validationStorage.AddError(UnknownUser, $"User with Id {userId} does not exist");
    }
}
