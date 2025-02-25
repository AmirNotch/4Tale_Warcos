namespace UserProfile.Validation;

public interface IValidationStorage
{
    void AddError(ErrorCode errorCode, string errorMessage);
    bool IsValid { get; }
    (ErrorCode, string) GetError();
    void Clear();
}