namespace UserProfile.Validation;

public class ValidationStorage : IValidationStorage
{
    private List<(ErrorCode, string)> _storage;

    public ValidationStorage()
    {
        _storage = [];
    }

    public void AddError(ErrorCode errorCode, string errorMessage)
    {
        _storage.Add((errorCode, errorMessage));
    }

    public bool IsValid {
        get { return _storage.Count == 0; }
    }

    public void Clear()
    {
        _storage.Clear();
    }

    public (ErrorCode, string) GetError()
    {
        return _storage[0];
    }

    public override string ToString()
    {
        string payload = string.Join(",", _storage.Select(pair => $"{{\"{pair.Item1}\":\"{pair.Item2}\"}}").ToList());
        return $"[{payload}]";
    }
}