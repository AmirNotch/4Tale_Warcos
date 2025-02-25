namespace UserProfile.Models.db;

public class UserLevelReward
{
    public Guid UserId { get; set; }
    public virtual UserProfile UserProfile { get; set; }
    
    public Guid RewardId { get; set; }
    public virtual LevelReward LevelReward { get; set; }

    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
}
