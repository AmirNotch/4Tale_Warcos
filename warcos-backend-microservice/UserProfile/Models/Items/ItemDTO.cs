namespace UserProfile.Models.Items;

public class ItemDTO
{
    public Guid ItemId { get; set; }
    public string UnrealId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}