using UserProfile.Validation;

using static UserProfile.Validation.ErrorCode;

namespace UserProfile.Util;

public class ValidationUtils
{
    public static void AddUnknownUserError(IValidationStorage validationStorage, Guid userId)
    {
        validationStorage.AddError(UnknownUser, $"User with Id {userId} does not exist");
    }
    
    public static void AddUserAlreadyExistsError(IValidationStorage validationStorage, Guid userId)
    {
        validationStorage.AddError(UserAlreadyExists, $"User with Id {userId} does not exist");
    }
}