namespace UserProfile.Models.db;

public class Item
{
    public Guid ItemId { get; set; } = Guid.NewGuid();
    public string UnrealId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Достижения и награды, связанные с предметом
    public ICollection<Achievement> AchievementRewards { get; set; }
    public ICollection<LevelReward> LevelRewards { get; set; }
}