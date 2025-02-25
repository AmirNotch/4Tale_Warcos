using Common.Validation.Attributes;

namespace UserProfile.Models.Items;

public class ItemRequest
{
    [ValidNotEmptyString]
    public string UnrealId { get; set; }
}