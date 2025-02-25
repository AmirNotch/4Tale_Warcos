using Common.Validation.Attributes;

namespace UserProfile.Models.Items;

public class ItemUpdateRequest
{
    [ValidGuid]
    public Guid ItemId { get; set; }
    [ValidNotEmptyString]
    public string UnrealId { get; set; }
}